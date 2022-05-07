using CurriculumDomain.Features.Subjects.GetLessonsBrief.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntitiesUnits = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetLessonsBrief.CQRS.Query;
public class GetLessonsBriefBySubjectIdQueryHandler : IRequestHandler<GetLessonsBriefBySubjectIdQuery, CommitResults<LessonsBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetLessonsBriefBySubjectIdQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GetLessonsBriefBySubjectIdQuery
    public async Task<CommitResults<LessonsBriefResponse>> Handle(GetLessonsBriefBySubjectIdQuery request, CancellationToken cancellationToken)
        => new CommitResults<LessonsBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntitiesUnits.Unit>()
                               .Where(a => a.SubjectId.Equals(request.SubjectId))
                               .Include(a => a.Lessons)
                               .SelectMany(a => a.Lessons)
                               .Select(a => new LessonsBriefResponse
                               {
                                   Id = a.Id,
                                   Name = a.Name,
                                   Points = a.Points,
                               })
                               .ToListAsync(cancellationToken)
        };
}