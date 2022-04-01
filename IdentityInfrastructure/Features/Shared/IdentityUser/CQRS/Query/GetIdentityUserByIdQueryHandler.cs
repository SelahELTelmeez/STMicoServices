using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using DomainEntities = IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.Shared.IdentityUser.CQRS.Query;
public class GetIdentityUserByIdQueryHandler : IRequestHandler<GetIdentityUserByIdQuery, DomainEntities.IdentityUser>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserByIdQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<DomainEntities.IdentityUser> Handle(GetIdentityUserByIdQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        return await _dbContext.Set<DomainEntities.IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(request.IdentityUserId), cancellationToken);
    }
}