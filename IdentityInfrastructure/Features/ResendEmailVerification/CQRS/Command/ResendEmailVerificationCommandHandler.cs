﻿using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
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
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // user data is not Exist, try to sign in instead.
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
                    ErrorCode = "X0006",
                    ErrorMessage = _resourceJsonManager["X0006"], 
                    ResultType = ResultType.NotFound,
                };
            }

            //3.0 Disable All Previous Resend Email Verification Code.
            List<IdentityActivation> identityActivations =  await _dbContext.Set<IdentityActivation>().Where(a => a.IsActive && a.ActivationType == ActivationType.Email).ToListAsync(cancellationToken);
            if (identityActivations.Any())
            {
                foreach (IdentityActivation activation in identityActivations)
                {
                    activation.RevokedOn = DateTime.UtcNow;
                    _dbContext.Set<IdentityActivation>().Update(activation);
                }
            }
            IdentityActivation identityActivation = new IdentityActivation
            {
                ActivationType = ActivationType.Email,
                Code = UtilityGenerator.GetOTP(4).ToString(),
                IdentityUserId = identityUser.Id,
            };
            _dbContext.Set<IdentityActivation>().Add(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            bool result = await _notificationEmailService.SendEmailAsync(new EmailNotificationModel
            {
                MailFrom = "noreply@selaheltelmeez.com",
                MailTo = identityUser.Email,
                MailSubject = "سلاح التلميذ - رمز التفعيل",
                IsBodyHtml = true,
                DisplayName = "سلاح التلميذ",
                MailToName = identityUser.FullName,
                MailBody = identityActivation.Code
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