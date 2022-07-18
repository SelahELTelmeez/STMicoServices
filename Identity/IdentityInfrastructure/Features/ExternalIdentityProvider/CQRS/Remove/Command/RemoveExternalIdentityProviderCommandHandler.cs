using Flaminco.CommitResult;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
using IdentityEntities.Entities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.ExternalIdentityProvider.CQRS.Remove.Command;
public class RemoveExternalIdentityProviderCommandHandler : IRequestHandler<RemoveExternalIdentityProviderCommand, ICommitResult>
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

    public async Task<ICommitResult> Handle(RemoveExternalIdentityProviderCommand request, CancellationToken cancellationToken)
    {
        //1.0 Start get facebook of user to the databse.
        DomainEntities.ExternalIdentityProvider? externalIdentityProvider = await _dbContext.Set<DomainEntities.ExternalIdentityProvider>()
                                                     .FirstOrDefaultAsync(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                                                                a.Name.Equals(request.RemoveExternalIdentityProviderRequest.Name), cancellationToken);

        if (externalIdentityProvider == null)
        {
            return ResultType.NotFound.GetCommitResult("XIDN0016", _resourceJsonManager["XIDN0016"]);
        }
        else
        {
            //2.0 Remove here.
            _dbContext.Set<DomainEntities.ExternalIdentityProvider>().Remove(externalIdentityProvider);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}