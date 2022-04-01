using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityInfrastructure.Features.Shared.IdentityUser.CQRS.Query;
public class GetIdentityUserByEmailAndPasswordQueryHandler : IRequestHandler<GetIdentityUserByEmailAndPasswordQuery, DomainEntities.IdentityUser>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserByEmailAndPasswordQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<DomainEntities.IdentityUser> Handle(GetIdentityUserByEmailAndPasswordQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        return await _dbContext.Set<DomainEntities.IdentityUser>()
                               .SingleOrDefaultAsync(a => a.Email.Equals(request.Email) && a.PasswordHash == request.PasswordHash, cancellationToken);
    }
}