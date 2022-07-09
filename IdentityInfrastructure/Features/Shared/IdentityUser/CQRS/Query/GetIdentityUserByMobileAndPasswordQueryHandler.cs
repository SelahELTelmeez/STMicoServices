using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Shared.IdentityUser.CQRS.Query;
public class GetIdentityUserByMobileAndPasswordQueryHandler : IRequestHandler<GetIdentityUserByMobileAndPasswordQuery, DomainEntities.IdentityUser>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserByMobileAndPasswordQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<DomainEntities.IdentityUser?> Handle(GetIdentityUserByMobileAndPasswordQuery request, CancellationToken cancellationToken)
        => await _dbContext.Set<DomainEntities.IdentityUser>()
                           .FirstOrDefaultAsync(a => a.MobileNumber.Equals(request.MobileNumber) && a.PasswordHash == request.PasswordHash, cancellationToken);
}