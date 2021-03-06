using Flaminco.CommitResult;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
using IdentityEntities.Entities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.ExternalIdentityProvider.CQRS.Add.Command;
public class AddExternalIdentityProviderCommandHandler : IRequestHandler<AddExternalIdentityProviderCommand, ICommitResult>
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

    public async Task<ICommitResult> Handle(AddExternalIdentityProviderCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the facebook Id existance first, with the provided data.
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
                                                     .FirstOrDefaultAsync(a => a.Name!.Equals(request.AddExternalIdentityProviderRequest.Name) &&
                                                                                a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (externalIdentityProvider != null)
        {
            return ResultType.NotFound.GetCommitResult("XIDN0005", _resourceJsonManager["XIDN0005"]);
        }
        else
        {
            //2.0 Start Adding the facebook of user to the databse.
            DomainEntities.ExternalIdentityProvider addExternalIdentityProviderRequest = new DomainEntities.ExternalIdentityProvider
            {
                Identifierkey = request.AddExternalIdentityProviderRequest.ProviderId,
                Name = request.AddExternalIdentityProviderRequest.Name,
                IdentityUserId = _httpContextAccessor.GetIdentityUserId()
            };

            _dbContext.Set<DomainEntities.ExternalIdentityProvider>().Add(addExternalIdentityProviderRequest);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}