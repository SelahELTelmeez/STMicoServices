﻿using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
using IdentityEntities.Entities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.ExternalIdentityProvider.CQRS.Remove.Command;
public class RemoveExternalIdentityProviderCommandHandler : IRequestHandler<RemoveExternalIdentityProviderCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RemoveExternalIdentityProviderCommandHandler(STIdentityDbContext dbContext,
                                                        IWebHostEnvironment configuration,
                                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(RemoveExternalIdentityProviderCommand request, CancellationToken cancellationToken)
    {
        //1.0 Start get facebook of user to the databse.
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
                                                     .SingleOrDefaultAsync(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                                                                a.Name.Equals(request.RemoveExternalIdentityProviderRequest.Name), cancellationToken);

        if (externalIdentityProvider == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"],
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Remove here.
            _dbContext.Set<DomainEntities.ExternalIdentityProvider>().Remove(externalIdentityProvider);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}