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
    public async Task<DomainEntities.IdentityUser> Handle(GetIdentityUserByMobileAndPasswordQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        return await _dbContext.Set<DomainEntities.IdentityUser>()
                               .SingleOrDefaultAsync(a => a.MobileNumber.Equals(request.MobileNumber) && a.PasswordHash == request.PasswordHash, cancellationToken);
    }
}