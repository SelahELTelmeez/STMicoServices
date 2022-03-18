using IdentityDomain.Features.GetGovernorates.CQRS.Query;
using IdentityDomain.Features.GetGovernorates.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Locations;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.GetGovernorates.CQRS.Query;

public class GetGovernoratesQueryHandler : IRequestHandler<GetGovernoratesQuery, CommitResult<List<GovernorateResponseDTO>>>
{
    private readonly STIdentityDbContext _dbContext;
    public GetGovernoratesQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult<List<GovernorateResponseDTO>>> Handle(GetGovernoratesQuery request, CancellationToken cancellationToken)
    {
        return new CommitResult<List<GovernorateResponseDTO>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Governorate>().ProjectToType<GovernorateResponseDTO>().ToListAsync(cancellationToken)
        };
    }
}


