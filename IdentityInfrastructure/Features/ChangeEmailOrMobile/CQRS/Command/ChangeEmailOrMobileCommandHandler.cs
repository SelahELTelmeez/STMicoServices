﻿using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using IdentityDomain.Models;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ChangeEmailOrMobile.CQRS.Command;
public class ChangeEmailOrMobileCommandHandler : IRequestHandler<ChangeEmailOrMobileCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotificationService _notificationEmailService;

    public ChangeEmailOrMobileCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager,
                                             INotificationService notificationEmailService, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _httpContextAccessor = httpContextAccessor;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<CommitResult> Handle(ChangeEmailOrMobileCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)) &&
                                                                                                    a.PasswordHash.Equals(request.ChangeEmailOrMobileRequest.Password), cancellationToken);

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
            //2.0 Start updating user data in the databse.
            identityUser.Email = request.ChangeEmailOrMobileRequest.NewEmail;
            identityUser.MobileNumber = request.ChangeEmailOrMobileRequest.NewMobileNumber;
            _dbContext.Set<IdentityUser>().Update(identityUser);

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
                MailBody = identityActivation.Code
            }, cancellationToken);

            _ = _notificationEmailService.SendSMSAsync(new SMSNotificationModel
            {
                MobileNumber = identityUser.MobileNumber,
                OTPCode = identityActivation.Code
            }, cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}