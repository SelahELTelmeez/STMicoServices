using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ForgetPassword.CQRS.Command;
public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, CommitResult>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationEmailService _notificationEmailService;

    public ForgetPasswordCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, INotificationEmailService notificationEmailService)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<CommitResult> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        //1. Check if the user exists with basic data entry.
        // Check by email first.
        if (!string.IsNullOrEmpty(request.ForgetPasswordRequest.Email))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email!.Equals(request.ForgetPasswordRequest.Email));
            if (identityUser == null)
            {
                return new CommitResult
                {
                    ErrorCode = "X0003",
                    ErrorMessage = _resourceJsonManager["X0003"], // User data Not Exist, try to sign in instead.
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                await SentForgetPasswordURL(identityUser);

                return new CommitResult
                {
                    ResultType = ResultType.Ok
                };
            }
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.ForgetPasswordRequest.MobileNumber))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber == request.ForgetPasswordRequest.MobileNumber);
            if (identityUser == null)
            {
                return new CommitResult
                {
                    ErrorCode = "X0003",
                    ErrorMessage = _resourceJsonManager["X0003"], // User data Not Exist, try to sign in instead.
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                await SentForgetPasswordURL(identityUser);

                return new CommitResult
                {
                    ResultType = ResultType.Ok
                };
            }
        }
        return new CommitResult
        {
            ErrorCode = "X0003",
            ErrorMessage = _resourceJsonManager["X0003"], // Success To Send Forget Password Email, try to sign in instead.
            ResultType = ResultType.Invalid,
        };
    }

    public async Task<bool> SentForgetPasswordURL(IdentityUser identityUser)
    {
        return await _notificationEmailService.SendAsync(new EmailNotificationModel
        {

            MailTo = identityUser.Email,
            IsBodyHtml = true,
            MailBody = "",
            MailSubject = ""
        }) ;
    }
}