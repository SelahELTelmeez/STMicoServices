﻿using IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    private readonly IMediator _mediator;

    public ResendMobileVerificationCommandHandler(STIdentityDbContext dbContext,
                                                  IWebHostEnvironment webEnv,
                                                  IHttpContextAccessor httpContextAccessor,
                                                  INotificationService notificationService,
                                                  IConfiguration configuration, IMediator mediator)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(webEnv.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _notificationService = notificationService;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _mediator = mediator;
    }

    public async Task<CommitResult> Handle(ResendMobileVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "XIDN0001",
                ErrorMessage = _resourceJsonManager["XIDN0001"], // facebook data is Exist, try to sign in instead.
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
                    ErrorCode = "XIDN0014",
                    ErrorMessage = _resourceJsonManager["XIDN0014"]
                };
            }

            // Check SMS Limit per day.
            await _dbContext.Entry(identityUser).Collection(a => a.Activations).LoadAsync(cancellationToken);

            if (identityUser.Activations.Where(a => (DateTime.UtcNow.StartOfDay() < a.CreatedOn) && (a.CreatedOn < DateTime.UtcNow.EndOfDay()) && a.ActivationType == ActivationType.Mobile).Count() >= int.Parse(_configuration["SMSSettings:ClientDailySMSLimit"]))
            {
                return new CommitResult
                {
                    ErrorCode = "XIDN0006", // Exceed the limit of SMS for today.
                    ErrorMessage = _resourceJsonManager["XIDN0006"],
                    ResultType = ResultType.Unauthorized
                };
            }

            //3.0 Disable All Previous Resend Email Verification Code.
            if (identityUser.Activations.Where(a => a.IsActive && a.ActivationType == ActivationType.Mobile).Any())
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
                    ErrorCode = "XIDN0013", // Couldn't send a SMS Message
                    ErrorMessage = _resourceJsonManager["XIDN0013"],
                    ResultType = ResultType.Invalid
                };
            }
        }

    }

}