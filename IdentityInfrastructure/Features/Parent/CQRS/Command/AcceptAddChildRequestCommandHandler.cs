﻿using IdentityDomain.Features.Parent.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.HttpClients;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command;

public class AcceptAddChildRequestCommandHandler : IRequestHandler<AcceptChildInvitationCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _userId;
    private readonly NotifierClient _notifierClient;
    public AcceptAddChildRequestCommandHandler(STIdentityDbContext dbContext,
                                  IWebHostEnvironment configuration,
                                  IHttpContextAccessor httpContextAccessor,
                                  NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _userId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
    }
    public async Task<CommitResult> Handle(AcceptChildInvitationCommand request, CancellationToken cancellationToken)
    {
        IdentityRelation? identityRelation = await _dbContext.Set<IdentityRelation>().SingleOrDefaultAsync(a => a.PrimaryId == request.AddChildInvitationRequest.ParentId && a.SecondaryId == _userId && a.RelationType == RelationType.ParentToKid);
        if (identityRelation != null)
        {
            return new CommitResult
            {
                ResultType = ResultType.Duplicated,
                ErrorCode = "X0000",
                ErrorMessage = "X0000"
            };
        }

        IdentityRelation IdentityRelation = new IdentityRelation
        {
            RelationType = RelationType.ParentToKid,
            PrimaryId = request.AddChildInvitationRequest.ParentId,
            SecondaryId = _userId
        };

        _dbContext.Set<IdentityRelation>().Add(IdentityRelation);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _ = _notifierClient.SetAsInActiveInvitationAsync(request.AddChildInvitationRequest.InvitationId, cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok,
        };
    }
}

