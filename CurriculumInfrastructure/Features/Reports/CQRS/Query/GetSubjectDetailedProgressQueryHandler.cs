using CurriculumDomain.Features.Reports.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Subjects;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Reports.CQRS.Query;

public class GetSubjectDetailedProgressQueryHandler : IRequestHandler<GetSubjectDetailedProgressQuery, CommitResult<DetailedProgressResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IDistributedCache _cahce;

    public GetSubjectDetailedProgressQueryHandler(CurriculumDbContext dbContext,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor,
                                                  IDistributedCache cahce)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _cahce = cahce;
    }
    public async Task<CommitResult<DetailedProgressResponse>> Handle(GetSubjectDetailedProgressQuery request, CancellationToken cancellationToken)
    {

        DetailedProgressResponse? cachedDetailedProgressResponse = await _cahce.GetFromCacheAsync<string, DetailedProgressResponse>(request.SubjectId, "Curriculum-GetSubjectDetailedProgress", cancellationToken);

        if (cachedDetailedProgressResponse == null)
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
            cachedDetailedProgressResponse = new DetailedProgressResponse
            {
                SubjectId = request.SubjectId,
                SubjectName = subject.ShortName,
                TotalSubjectScore = subject.Units.SelectMany(a => a.Lessons).SelectMany(a => a.Clips).Where(a => a.Usability >= 2).Sum(a => a.Points) ?? 0,
                UnitProgresses = UnitMapper()
            };

            await _cahce.SaveToCacheAsync<string, DetailedProgressResponse>(request.SubjectId, cachedDetailedProgressResponse, "Curriculum-GetSubjectDetailedProgress", cancellationToken);

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
        }



        return new CommitResult<DetailedProgressResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedDetailedProgressResponse
        };
    }
}
