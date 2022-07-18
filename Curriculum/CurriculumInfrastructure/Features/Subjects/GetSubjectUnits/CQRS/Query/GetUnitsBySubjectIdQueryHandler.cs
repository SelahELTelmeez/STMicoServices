using CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
using CurriculumEntites.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;
using DomainEntities = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectUnits.CQRS.Query;
public class GetUnitsBySubjectIdQueryHandler : IRequestHandler<GetUnitsBySubjectIdQuery, CommitResults<UnitResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;
    public GetUnitsBySubjectIdQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CommitResults<UnitResponse>> Handle(GetUnitsBySubjectIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<UnitResponse>? cachedUnitResponses = await _cache.GetFromCacheAsync<string, IEnumerable<UnitResponse>>(request.SubjectId, "Curriculum-GetUnitsBySubjectId", cancellationToken);

        if (cachedUnitResponses == null)
        {
            cachedUnitResponses = await _dbContext.Set<DomainEntities.Unit>()
                                    .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsShow == true)
                                    .OrderBy(a => a.Sort)
                                    .Include(a => a.Lessons)
                                    .ProjectToType<UnitResponse>()
                                    .ToListAsync(cancellationToken);

            await _cache.SaveToCacheAsync(request.SubjectId, cachedUnitResponses, "Curriculum-GetUnitsBySubjectId", cancellationToken);
        }

        return new CommitResults<UnitResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedUnitResponses
        };
    }

}