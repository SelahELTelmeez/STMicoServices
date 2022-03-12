using IdentityDomain.Features.ChangePassword.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ChangePassword.CQRS.Command;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationEmailService;

    public ChangePasswordCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager,
                                        INotificationService notificationEmailService, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _httpContextAccessor = httpContextAccessor;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<CommitResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)) &&
                                                                                                    a.PasswordHash.Equals(request.ChangePasswordRequest.OldPassword), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], 
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Start updating user data in the databse.
            // Add Mapping here.
            identityUser.PasswordHash = request.ChangePasswordRequest.NewPassword;
            _dbContext.Set<IdentityUser>().Update(identityUser);

            //3.0 Send Email of Change Passsword.
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
                MailBody = identityActivation.Code
            }, cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}