using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Shared.IdentityUser.CQRS.Query;
public class GetIdentityUserByIdQueryHandler : IRequestHandler<GetIdentityUserByIdQuery, DomainEntities.IdentityUser>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserByIdQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<DomainEntities.IdentityUser?> Handle(GetIdentityUserByIdQuery request, CancellationToken cancellationToken)
        => await _dbContext.Set<DomainEntities.IdentityUser>().FirstOrDefaultAsync(a => a.Id.Equals(request.IdentityUserId), cancellationToken);

}