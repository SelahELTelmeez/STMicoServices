using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ResendEmailVerification.CQRS.Command;
public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationEmailService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResendEmailVerificationCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager, 
                                                 INotificationService notificationEmailService, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationEmailService = notificationEmailService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"], // user data is not Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Check if email is null
            if (identityUser.Email == null)
            {
                return new CommitResult
                {
                    ErrorCode = "X0005",
                    ErrorMessage = _resourceJsonManager["X0005"], // Email is null, try to sign in instead.
                    ResultType = ResultType.NotFound,
                };
            }

            //3.0 Resend Email Verification Code.
            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = ActivationType.Email,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _ = _notificationEmailService.SendEmailAsync(new EmailNotificationModel
            {
                MailFrom = "noreply@selaheltelmeez.com",
                MailTo = identityUser.Email,
                MailSubject = "سلاح التلميذ - رمز التفعيل",
                IsBodyHtml = true,
                DisplayName = "سلاح التلميذ",
                MailToName = identityUser.FullName,
                MailBody = UtilityGenerator.GetOTP(4).ToString()
            }, cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}