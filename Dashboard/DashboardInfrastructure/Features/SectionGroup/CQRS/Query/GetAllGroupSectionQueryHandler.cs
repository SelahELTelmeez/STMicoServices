using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Query;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEntities = DashboardEntity.Entities;

namespace DashboardInfrastructure.Features.SectionGroup.CQRS.Query;

public class GetAllGroupSectionQueryHandler : IRequestHandler<GetAllGroupSectionQuery, ICommitResults<SectionGroupResponse>>
{
    private readonly DashboardDbContext _dbContext;

    public GetAllGroupSectionQueryHandler(DashboardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICommitResults<SectionGroupResponse>> Handle(GetAllGroupSectionQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<DomainEntities.SectionGroup>()
                                                                   .Include(a => a.Sections)
                                                                   .Select(a => new SectionGroupResponse
                                                                   {
                                                                       GroupName = a.Name,
                                                                       GroupId = a.Id,
                                                                       Sections = a.Sections.Select(b => new SectionResponse
                                                                       {
                                                                           Id = b.Id,
                                                                           Description = b.Description,
                                                                           Name = b.Name,
                                                                           Thumbnail = b.Thumbnail,
                                                                           Type = b.Type,
                                                                           Url = b.Url,
                                                                       })
                                                                   }).ToListAsync(cancellationToken));
    }
}