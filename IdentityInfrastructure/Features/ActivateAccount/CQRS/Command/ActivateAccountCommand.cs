using IdentityDomain.Features.ActivateAccount.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ActivateAccount.CQRS.Command;

public class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand, CommitResult>
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
    public async Task<CommitResult> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id == _httpContextAccessor.GetIdentityUserId(), cancellationToken);
        if (identityUser == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "XIDN0001",
                ErrorMessage = "XIDN0001"
            };
        }
        identityUser.IsMobileVerified = true;
        identityUser.IsEmailVerified = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}


