﻿using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ResendEmailVerification.CQRS.Command;
public class ResendEmailVerificationCommandHandler : IRequestHandler<ResendEmailVerificationCommand, CommitResult>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationEmailService _notificationEmailService;

    public ResendEmailVerificationCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, INotificationEmailService notificationEmailService)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _notificationEmailService = notificationEmailService;
    }

    public async Task<CommitResult> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(request.ResendEmailVerificationRequest.IdentityUserId), cancellationToken);

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
            //2.0 Send Email Verification Code.
            // Add Mapping here.
            //await _notificationEmailService.SendAsync(new IdentityDomain.Models.EmailNotificationModel
            //{
            //    MailTo = identityUser.Email,
            //    IsBodyHtml = true,
            //    MailBody = "",
            //    MailSubject = "",

            //});

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}