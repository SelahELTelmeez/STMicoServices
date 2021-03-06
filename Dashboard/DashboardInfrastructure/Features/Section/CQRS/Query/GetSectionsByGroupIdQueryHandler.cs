using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Query;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.Section.CQRS.Query;

public class GetSectionsByGroupIdQueryHandler : IRequestHandler<GetSectionsByGroupIdQuery, ICommitResults<SectionResponse>>
{
    private readonly DashboardDbContext _dbContext;

    public GetSectionsByGroupIdQueryHandler(DashboardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICommitResults<SectionResponse>> Handle(GetSectionsByGroupIdQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<DomainEntities.Section>().Where(a => a.SectionGroupId.Equals(request.GroupId))
                                                                    .ProjectToType<SectionResponse>()
                                                                    .ToListAsync(cancellationToken));
    }
}