using Flaminco.CommitResult;
using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.ChangeEmailOrMobile.CQRS.Command;
public class ChangeEmailOrMobileCommandHandler : IRequestHandler<ChangeEmailOrMobileCommand, ICommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationEmailService;

    public ChangeEmailOrMobileCommandHandler(STIdentityDbContext dbContext,
                                             IWebHostEnvironment configuration,
                                             IHttpContextAccessor httpContextAccessor,
                                             INotificationService notificationEmailService)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<ICommitResult> Handle(ChangeEmailOrMobileCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.Id.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                                                                                    a.PasswordHash.Equals(request.ChangeEmailOrMobileRequest.Password.Encrypt(true)), cancellationToken);

        if (identityUser == null)
        {
            return ResultType.NotFound.GetCommitResult("XIDN0001", _resourceJsonManager["XIDN0001"]);
        }

        bool isEmailUsed = !string.IsNullOrEmpty(request.ChangeEmailOrMobileRequest.NewEmail);

        // Check if new data is already exists.
        //2.0 Start adding the temp values in the databse.
        if (isEmailUsed)
        {
            if (await _dbContext.Set<IdentityUser>().AnyAsync(a => a.Email == request.ChangeEmailOrMobileRequest.NewEmail.Trim().ToLower(), cancellationToken))
            {
                return ResultType.NotFound.GetCommitResult("XIDN0002", _resourceJsonManager["XIDN0002"]);
            }
            _dbContext.Set<IdentityTemporaryValueHolder>().Add(new IdentityTemporaryValueHolder
            {
                Name = "Email",
                Value = request.ChangeEmailOrMobileRequest.NewEmail.ToLower(),
                IdentityUserId = identityUser.Id
            });
        }
        else
        {
            if (await _dbContext.Set<IdentityUser>().AnyAsync(a => a.MobileNumber.Equals(request.ChangeEmailOrMobileRequest.NewMobileNumber), cancellationToken))
            {
                return ResultType.NotFound.GetCommitResult("XIDN0003", _resourceJsonManager["XIDN0003"]);
            }
            _dbContext.Set<IdentityTemporaryValueHolder>().Add(new IdentityTemporaryValueHolder
            {
                Name = "Mobile",
                Value = request.ChangeEmailOrMobileRequest.NewMobileNumber,
                IdentityUserId = identityUser.Id
            });
        }

        //3.0 Resend Email Verification Code.
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
            _ = _notificationEmailService.SendEmailAsync(new EmailNotificationModel
            {
                MailFrom = "noreply@selaheltelmeez.com",
                MailTo = request.ChangeEmailOrMobileRequest.NewEmail,
                MailSubject = "سلاح التلميذ - تغير البريد الإلكترونى",
                IsBodyHtml = true,
                DisplayName = "سلاح التلميذ",
                MailToName = identityUser.FullName,
                MailBody = identityActivation.Code // Message call تم تغيير البريد الالكترونى بنجاح 
            }, cancellationToken);
        }
        else
        {
            _ = _notificationEmailService.SendSMSAsync(new SMSNotificationModel
            {
                Mobile = request.ChangeEmailOrMobileRequest.NewMobileNumber,
                Code = identityActivation.Code
            }, cancellationToken);

        }

        return ResultType.Ok.GetCommitResult();
    }
}