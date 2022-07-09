using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResultHandler;

namespace IdentityInfrastructure.Features.ForgetPassword.CQRS.Command;
public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;
    public ForgetPasswordCommandHandler(STIdentityDbContext dbContext,
                                        IWebHostEnvironment WebEnv,
                                        IHttpContextAccessor httpContextAccessor,
                                        INotificationService notificationService,
                                        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(WebEnv.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _notificationService = notificationService;
        _configuration = configuration;
    }

    public async Task<CommitResult> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        //1. Check if the user exists with basic data entry.
        // Check by email first.
        bool isEmailUsed = !string.IsNullOrWhiteSpace(request.ForgetPasswordRequest.Email);
        IdentityUser? identityUser;
        if (isEmailUsed)
        {
            identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.Email.Equals(request.ForgetPasswordRequest.Email), cancellationToken);
        }
        else
        {
            identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.MobileNumber.Equals(request.ForgetPasswordRequest.MobileNumber), cancellationToken);
        }

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "XIDN0001",
                ErrorMessage = _resourceJsonManager["XIDN0001"], // User data Not Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            await _dbContext.Entry(identityUser).Collection(a => a.Activations).LoadAsync(cancellationToken);

            if (!isEmailUsed)
            {
                // Check SMS Limit per day.

                if (identityUser.Activations.Where(a => (DateTime.UtcNow.StartOfDay() < a.CreatedOn) && (a.CreatedOn < DateTime.UtcNow.EndOfDay()) && a.ActivationType == ActivationType.Mobile).Count() >= int.Parse(_configuration["SMSSettings:ClientDailySMSLimit"]))
                {
                    return new CommitResult
                    {
                        ErrorCode = "XIDN0006", // Exceed the limit of SMS for today.
                        ErrorMessage = _resourceJsonManager["XIDN0006"],
                        ResultType = ResultType.Unauthorized
                    };
                }
            }

            //3.0 Disable All Previous Resend Email Verification Code.
            if (identityUser.Activations.Where(a => a.IsActive && a.ActivationType == ActivationType.Email).Any())
            {
                foreach (IdentityActivation activation in identityUser.Activations)
                {
                    activation.RevokedOn = DateTime.UtcNow;
                    _dbContext.Set<IdentityActivation>().Update(activation);
                }
            }

            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = isEmailUsed ? ActivationType.Email : ActivationType.Mobile,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            if (isEmailUsed)
            {
                _ = _notificationService.SendEmailAsync(new EmailNotificationModel
                {
                    MailFrom = "noreply@selaheltelmeez.com",
                    MailTo = identityUser.Email,
                    MailSubject = "سلاح التلميذ - رمز التفعيل",
                    IsBodyHtml = true,
                    DisplayName = "سلاح التلميذ",
                    MailToName = identityUser.FullName,
                    MailBody = identityActivation.Code
                }, cancellationToken);
            }
            else
            {
                _ = _notificationService.SendSMSAsync(new SMSNotificationModel
                {
                    Mobile = identityUser.MobileNumber,
                    Code = identityActivation.Code
                }, cancellationToken);
            }

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}