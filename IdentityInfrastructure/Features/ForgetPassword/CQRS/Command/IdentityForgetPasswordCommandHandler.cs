using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResultHandler;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityInfrastructure.Features.ForgetPassword.CQRS.Command;
public class IdentityForgetPasswordCommandHandler : IRequestHandler<IdentityForgetPasswordCommand, CommitResult<bool>>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationEmailService _notificationEmailService;

    public IdentityForgetPasswordCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, INotificationEmailService notificationEmailService)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<CommitResult<bool>> Handle(IdentityForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        //1. Check if the user exists with basic data entry.
        // Check by email first.
        if (!string.IsNullOrEmpty(request.IdentityForgetPasswordRequest.Email))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Email!.Equals(request.IdentityForgetPasswordRequest.Email));
            if (identityUser == null)
            {
                return new CommitResult<bool>
                {
                    ErrorCode = "X0003",
                    ErrorMessage = _resourceJsonManager["X0003"], // User data Not Exist, try to sign in instead.
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                await SentForgetPasswordURL(identityUser);

                return new CommitResult<bool>
                {
                    ResultType = ResultType.Ok,
                    Value = true
                };
            }
        }
        // check by mobile number
        if (!string.IsNullOrEmpty(request.IdentityForgetPasswordRequest.MobileNumber))
        {
            IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.MobileNumber == request.IdentityForgetPasswordRequest.MobileNumber);
            if (identityUser == null)
            {
                return new CommitResult<bool>
                {
                    ErrorCode = "X0003",
                    ErrorMessage = _resourceJsonManager["X0003"], // User data Not Exist, try to sign in instead.
                    ResultType = ResultType.NotFound,
                };
            }
            else
            {
                await SentForgetPasswordURL(identityUser);

                return new CommitResult<bool>
                {
                    ResultType = ResultType.Ok,
                    Value = true
                };
            }
        }
        return new CommitResult<bool>
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