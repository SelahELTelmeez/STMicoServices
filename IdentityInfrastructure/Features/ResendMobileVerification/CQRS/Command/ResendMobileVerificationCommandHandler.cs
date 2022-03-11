using IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.MobileVerification.CQRS.Command;
public class ResendMobileVerificationCommandHandler : IRequestHandler<ResendMobileVerificationCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResendMobileVerificationCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager,
                                                  INotificationService notificationService, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationService = notificationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(ResendMobileVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 ReSend Email Verification Code.
            if (string.IsNullOrEmpty(identityUser.MobileNumber))
            {
                return new CommitResult
                {
                    ErrorCode = "X0000",
                    ErrorMessage = _resourceJsonManager["X0000"]
                };
            }
            // Check SMS Limit per day.
            await _dbContext.Entry(identityUser).Collection(a => a.Activations).LoadAsync(cancellationToken);

            if (identityUser.Activations.Where(a => (DateTime.UtcNow > a.CreatedOn) && (DateTime.UtcNow < DateTime.UtcNow.EndOfDay())).Count() >= 3)
            {
                return new CommitResult
                {
                    ErrorCode = "X0000", // Exceed the limit of SMS for today.
                    ErrorMessage = _resourceJsonManager["X0000"],
                    ResultType = ResultType.Unauthorized
                };
            }

            // Generate OTP
            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = ActivationType.Mobile,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);

            await _dbContext.SaveChangesAsync();

            bool result = await _notificationService.SendSMSAsync(new SMSNotificationModel
            {
                MobileNumber = identityUser.MobileNumber,
                OTPCode = identityActivation.Code
            }, cancellationToken);

            if (result)
            {
                return new CommitResult
                {
                    ResultType = ResultType.Ok
                };
            }
            else
            {
                return new CommitResult
                {
                    ErrorCode = "X0000", // Couldn't send a SMS Message
                    ErrorMessage = _resourceJsonManager["X0000"],
                    ResultType = ResultType.Invalid
                };
            }
        }

    }

}