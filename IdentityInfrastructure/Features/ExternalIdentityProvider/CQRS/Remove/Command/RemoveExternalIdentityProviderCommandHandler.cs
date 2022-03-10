﻿using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
using IdentityEntities.Entities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.ExternalIdentityProvider.CQRS.Remove.Command;
public class RemoveExternalIdentityProviderCommandHandler : IRequestHandler<RemoveExternalIdentityProviderCommand, CommitResult>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public RemoveExternalIdentityProviderCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult> Handle(RemoveExternalIdentityProviderCommand request, CancellationToken cancellationToken)
    {
        //1.0 Start get facebook of user to the databse.
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
                                                     .SingleOrDefaultAsync(a => a.IdentityUserId.Equals(request.RemoveExternalIdentityProviderRequest.IdentityUserId) && 
                                                                                a.Name.Equals(request.RemoveExternalIdentityProviderRequest.Name), cancellationToken);



        if (externalIdentityProvider == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0008",
                ErrorMessage = _resourceJsonManager["X0008"], // facebook data is not Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            // Remove here.
            _dbContext.Set<DomainEntities.ExternalIdentityProvider>().Remove(externalIdentityProvider);
            await _dbContext.SaveChangesAsync(cancellationToken);

            //2.0 Mapping Data.

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}