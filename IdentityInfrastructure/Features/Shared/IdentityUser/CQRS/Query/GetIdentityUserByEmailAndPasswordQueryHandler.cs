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
    public async Task<DomainEntities.IdentityUser?> Handle(GetIdentityUserByEmailAndPasswordQuery request, CancellationToken cancellationToken)
        => await _dbContext.Set<DomainEntities.IdentityUser>()
                           .SingleOrDefaultAsync(a => a.Email.Equals(request.Email) && a.PasswordHash == request.PasswordHash, cancellationToken);
}