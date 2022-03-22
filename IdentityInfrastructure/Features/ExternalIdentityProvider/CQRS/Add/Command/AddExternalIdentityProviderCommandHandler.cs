using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
using IdentityEntities.Entities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.ExternalIdentityProvider.CQRS.Add.Command;
public class AddExternalIdentityProviderCommandHandler : IRequestHandler<AddExternalIdentityProviderCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddExternalIdentityProviderCommandHandler(STIdentityDbContext dbContext,
                                                     IWebHostEnvironment configuration,
                                                     IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(AddExternalIdentityProviderCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the facebook Id existance first, with the provided data.
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
                                                     .SingleOrDefaultAsync(a => a.Name!.Equals(request.AddExternalIdentityProviderRequest.Name) &&
                                                                                a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (externalIdentityProvider != null)
        {
            return new CommitResult
            {
                ErrorCode = "X0003",
                ErrorMessage = _resourceJsonManager["X0003"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Start Adding the facebook of user to the databse.
            DomainEntities.ExternalIdentityProvider addExternalIdentityProviderRequest = request.AddExternalIdentityProviderRequest.Adapt<DomainEntities.ExternalIdentityProvider>();
            addExternalIdentityProviderRequest.IdentityUserId = _httpContextAccessor.GetIdentityUserId().Value;
            _dbContext.Set<DomainEntities.ExternalIdentityProvider>().Add(addExternalIdentityProviderRequest);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}