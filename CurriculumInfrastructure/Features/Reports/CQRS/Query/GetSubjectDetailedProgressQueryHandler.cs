using CurriculumDomain.Features.Reports.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Subjects;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Reports.CQRS.Query;

public class GetSubjectDetailedProgressQueryHandler : IRequestHandler<GetSubjectDetailedProgressQuery, CommitResult<DetailedProgressResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetSubjectDetailedProgressQueryHandler(CurriculumDbContext dbContext,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult<DetailedProgressResponse>> Handle(GetSubjectDetailedProgressQuery request, CancellationToken cancellationToken)
    {
        Subject? subject = await _dbContext.Set<Subject>()
                                           .Where(a => a.Id == request.SubjectId)
                                           .Include(a => a.Units)
                                           .ThenInclude(a => a.Lessons)
                                           .ThenInclude(a => a.Clips)
                                           .FirstOrDefaultAsync(cancellationToken);

        if (subject == null)
        {
            return new CommitResult<DetailedProgressResponse>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "XCUR0004",
                ErrorMessage = _resourceJsonManager["XCUR0004"]
            };
        }

        IEnumerable<DetailedLessonProgress> LessonMapper(IEnumerable<Lesson> lessons)
        {
            foreach (var lesson in lessons)
            {
                yield return new DetailedLessonProgress
                {
                    LessonId = lesson.Id,
                    LessonName = lesson.Name,
                    TotalLessonScore = lesson.Clips.Where(a => a.Usability >= 2).Sum(a => a.Points) ?? 0
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
                TotalSubjectScore = subject.Units.SelectMany(a => a.Lessons).SelectMany(a => a.Clips).Where(a => a.Usability >= 2).Sum(a => a.Points) ?? 0,
                UnitProgresses = UnitMapper()
            }
        };
    }
}
