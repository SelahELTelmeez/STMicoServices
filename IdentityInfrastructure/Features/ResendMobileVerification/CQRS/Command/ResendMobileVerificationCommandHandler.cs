using IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResultHandler;

namespace IdentityInfrastructure.Features.MobileVerification.CQRS.Command;
public class ResendMobileVerificationCommandHandler : IRequestHandler<ResendMobileVerificationCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    public ResendMobileVerificationCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager,
                                                  INotificationService notificationService, IHttpContextAccessor httpContextAccessor,
                                                  IConfiguration configuration)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationService = notificationService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<CommitResult> Handle(ResendMobileVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // facebook data is Exist, try to sign in instead.
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
                    ErrorCode = "X0009",
                    ErrorMessage = _resourceJsonManager["X0009"]
                };
            }

            // Check SMS Limit per day.
            await _dbContext.Entry(identityUser).Collection(a => a.Activations).LoadAsync(cancellationToken);

            if (identityUser.Activations.Where(a => (DateTime.UtcNow.StartOfDay() < a.CreatedOn) && (a.CreatedOn < DateTime.UtcNow.EndOfDay()) && a.ActivationType == ActivationType.Mobile).Count() >= int.Parse(_configuration["SMSSettings:ClientDailySMSLimit"]))
            {
                return new CommitResult
                {
                    ErrorCode = "X0008", // Exceed the limit of SMS for today.
                    ErrorMessage = _resourceJsonManager["X0008"],
                    ResultType = ResultType.Unauthorized
                };
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

            // Generate OTP
            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = ActivationType.Mobile,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id,
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);

            await _dbContext.SaveChangesAsync(cancellationToken);

            bool result = await _notificationService.SendSMSAsync(new SMSNotificationModel
            {
                Mobile = identityUser.MobileNumber,
                Code = identityActivation.Code
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