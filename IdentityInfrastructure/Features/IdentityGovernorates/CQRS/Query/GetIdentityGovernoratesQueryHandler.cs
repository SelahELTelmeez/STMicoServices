using Flaminco.CommitResult;
using IdentityDomain.Features.IdentityGovernorates.CQRS.Query;
using IdentityDomain.Features.IdentityGovernorates.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Locations;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.IdentityGovernorates.CQRS.Query;

public class GetIdentityGovernoratesQueryHandler : IRequestHandler<GetIdentityGovernoratesQuery, ICommitResults<IdentityGovernorateResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    public GetIdentityGovernoratesQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ICommitResults<IdentityGovernorateResponse>> Handle(GetIdentityGovernoratesQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<Governorate>().ProjectToType<IdentityGovernorateResponse>().ToListAsync(cancellationToken));
    }
}


