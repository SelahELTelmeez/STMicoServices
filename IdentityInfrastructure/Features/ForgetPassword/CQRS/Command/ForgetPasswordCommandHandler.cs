using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ForgetPassword.CQRS.Command;
public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationService;

    public ForgetPasswordCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationService = notificationService;
    }

    public async Task<CommitResult> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        //1. Check if the user exists with basic data entry.
        // Check by email first.
        bool isEmailUsed = !string.IsNullOrWhiteSpace(request.ForgetPasswordRequest.Email);
        IdentityUser? identityUser;
        if (isEmailUsed)
        {
            identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email.Equals(request.ForgetPasswordRequest.Email));
        }
        else
        {
            identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber.Equals(request.ForgetPasswordRequest.MobileNumber));
        }

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0008",
                ErrorMessage = _resourceJsonManager["X0008"], // User data Not Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = isEmailUsed ? ActivationType.Email : ActivationType.Mobile,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);
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
            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}