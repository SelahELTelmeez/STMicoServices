using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
using IdentityEntities.Entities;
using JsonLocalizer;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.ExternalIdentityProvider.CQRS.Add.Command;
public class AddExternalIdentityProviderCommandHandler : IRequestHandler<AddExternalIdentityProviderCommand, CommitResult>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public AddExternalIdentityProviderCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult> Handle(AddExternalIdentityProviderCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the facebook Id existance first, with the provided data.
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
                                                     .SingleOrDefaultAsync(a => a.Name!.Equals(request.AddExternalIdentityProviderRequest.Name) &&
                                                                                a.IdentityUserId.Equals(request.AddExternalIdentityProviderRequest.IdentityUserId) , cancellationToken);

        if (externalIdentityProvider != null)
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
            //2.0 Start Adding the facebook of user to the databse.
            // Add Mapping here.
            _dbContext.Set<DomainEntities.ExternalIdentityProvider>().Add(request.AddExternalIdentityProviderRequest.Adapt<DomainEntities.ExternalIdentityProvider>());
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}