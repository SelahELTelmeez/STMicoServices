using CurriculumDomain.Features.Subjects.GetSubjectUnits.CQRS.Query;
using CurriculumEntites.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;
using DomainEntities = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectUnits.CQRS.Query;
public class GetUnitsBySubjectIdQueryHandler : IRequestHandler<GetUnitsBySubjectIdQuery, CommitResults<UnitResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    public GetUnitsBySubjectIdQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResults<UnitResponse>> Handle(GetUnitsBySubjectIdQuery request, CancellationToken cancellationToken)
        =>
        new CommitResults<UnitResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntities.Unit>()
                                    .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsShow == true)
                                    .OrderBy(a => a.Sort)
                                    .Include(a => a.Lessons)
                                    .ProjectToType<UnitResponse>()
                                    .ToListAsync(cancellationToken)
        };
}