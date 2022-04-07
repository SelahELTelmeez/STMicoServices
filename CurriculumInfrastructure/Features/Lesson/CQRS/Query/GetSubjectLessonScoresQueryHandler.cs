using CurriculumDomain.Features.Lesson.CQRS.Query;
using CurriculumDomain.Features.Lesson.DTO.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntitiesUnits = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Lesson.CQRS.Query;
public class GetSubjectLessonScoresQueryHandler : IRequestHandler<GetSubjectLessonScoresQuery, CommitResults<LessonResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetSubjectLessonScoresQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //TODO: Add CommitResult
    public async Task<CommitResults<LessonResponse>> Handle(GetSubjectLessonScoresQuery request, CancellationToken cancellationToken)
    {

        return new CommitResults<LessonResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntitiesUnits.Unit>()
                               .Where(a => a.SubjectId.Equals(request.SubjectId))
                               .Include(a => a.Lessons)
                               .SelectMany(a => a.Lessons)
                               .Select(a => new LessonResponse
                               {
                                   Id = a.Id,
                                   Name = a.Name,
                                   Ponits = a.Ponits,
                               })
                               .ToListAsync(cancellationToken)
        };
    }
}