using Flaminco.CommitResult;
using IdentityDomain.Features.ActivateAccount.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.ActivateAccount.CQRS.Command;

public class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand, ICommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ActivateAccountCommandHandler(STIdentityDbContext dbContext,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;

    }
    public async Task<ICommitResult> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.Id == _httpContextAccessor.GetIdentityUserId(), cancellationToken);
        if (identityUser == null)
        {
            return ResultType.NotFound.GetCommitResult("XIDN0001", _resourceJsonManager["XIDN0001"]);
        }
        identityUser.IsMobileVerified = true;
        identityUser.IsEmailVerified = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}


