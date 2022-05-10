using CurriculumDomain.Features.Subjects.GetDetailedProgress.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetDetailedProgress.CQRS.Query;

public class GetSubjectDetailedProgressQueryHandler : IRequestHandler<GetSubjectDetailedProgressQuery, CommitResult<DetailedProgressResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    public GetSubjectDetailedProgressQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult<DetailedProgressResponse>> Handle(GetSubjectDetailedProgressQuery request, CancellationToken cancellationToken)
    {
        Subject? subject = await _dbContext.Set<Subject>()
                                           .Where(a => a.Id == request.SubjectId)
                                           .Include(a => a.Units)
                                           .ThenInclude(a => a.Lessons)
                                           .ThenInclude(a => a.Clips)
                                           .SingleOrDefaultAsync(cancellationToken);


        IEnumerable<DetailedLessonProgress> LessonMapper(IEnumerable<Lesson> lessons)
        {
            foreach (var lesson in lessons)
            {
                yield return new DetailedLessonProgress
                {
                    LessonId = lesson.Id,
                    LessonName = lesson.Name,
                    TotalLessonScore = lesson.Clips.Sum(a => a.Points) ?? 0
                };
            }
            yield break; ;
        }

        IEnumerable<DetailedUnitProgress> UnitMapper()
        {
            foreach (var unit in subject.Units)
            {
                yield return new DetailedUnitProgress
                {
                    UnitId = unit.Id,
                    UnitName = unit.Title,
                    LessonProgresses = LessonMapper(unit.Lessons)
                };
            }
        }

        return new CommitResult<DetailedProgressResponse>
        {
            ResultType = ResultType.Ok,
            Value = new DetailedProgressResponse
            {
                SubjectId = request.SubjectId,
                SubjectName = subject.ShortName,
                TotalSubjectScore = subject.Units.SelectMany(a => a.Lessons).SelectMany(a => a.Clips).Sum(a => a.Points) ?? 0,
                UnitProgresses = UnitMapper()
            }
        };
    }
}
