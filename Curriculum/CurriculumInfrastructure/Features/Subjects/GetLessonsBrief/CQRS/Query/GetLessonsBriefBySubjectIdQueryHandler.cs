using CurriculumDomain.Features.Subjects.GetLessonsBrief.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using DomainEntitiesUnits = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetLessonsBrief.CQRS.Query;
public class GetLessonsBriefBySubjectIdQueryHandler : IRequestHandler<GetLessonsBriefBySubjectIdQuery, CommitResults<LessonQuizResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IDistributedCache _cache;

    public GetLessonsBriefBySubjectIdQueryHandler(CurriculumDbContext dbContext,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor,
                                                  IDistributedCache cache)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _cache = cache;
    }

    // GetLessonsBriefBySubjectIdQuery
    public async Task<CommitResults<LessonQuizResponse>> Handle(GetLessonsBriefBySubjectIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<LessonQuizResponse>? cachedLessonQuizResponses = await _cache.GetFromCacheAsync<string, IEnumerable<LessonQuizResponse>>(request.SubjectId, "Curriculum-GetLessonsBriefBySubjectId", cancellationToken);

        if (cachedLessonQuizResponses == null)
        {
            IEnumerable<DomainEntitiesUnits.Unit> units = await _dbContext.Set<DomainEntitiesUnits.Unit>()
                                                                     .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsShow == true)
                                                                     .Include(a => a.SubjectFK)
                                                                     .Include(a => a.Lessons)
                                                                     .ThenInclude(a => a.Clips)
                                                                     .Where(a => a.IsShow == true)
                                                                     .ToListAsync(cancellationToken);

            if (!units.Any())
            {
                return new CommitResults<LessonQuizResponse>
                {
                    ResultType = ResultType.Empty,
                    ErrorCode = "XCUR0007",
                    ErrorMessage = _resourceJsonManager["XCUR0007"]
                };
            }

            IEnumerable<LessonQuizResponse> Mapper()
            {
                foreach (Lesson lesson in units.SelectMany(a => a.Lessons).Where(a => a.IsShow == true))
                {
                    Clip? clip = lesson.Clips.FirstOrDefault(a => a.Type == ClipType.Quiz && a.Status == ClipStatus.Production);
                    if (clip != null)
                    {
                        yield return new LessonQuizResponse
                        {
                            Id = lesson.Id,
                            Name = getLessonName(lesson.Type ?? 0, lesson.ShortName, lesson.UnitFK.ShortName, request.SubjectId, lesson.UnitFK.SubjectFK.ShortName),
                            QuizClipId = clip.Id,
                            Points = lesson.Points.GetValueOrDefault()
                        };
                    }

                }
                yield break;
            }

            cachedLessonQuizResponses = Mapper();

            await _cache.SaveToCacheAsync(request.SubjectId, cachedLessonQuizResponses, "Curriculum-GetLessonsBriefBySubjectId", cancellationToken);
        }

        return new CommitResults<LessonQuizResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedLessonQuizResponses
        };
    }

    private string getLessonName(int lessonType, string lessonName, string unitName, string subjectId, string subjectName)
    {
        switch (lessonType)
        {
            case 1:
                return lessonName;
            case 2:
                if (subjectId.ToLower().LastOrDefault() == 'a')
                    return "مراجعة على " + unitName;
                else
                    return "Review on " + unitName;
            case 3:
                if (subjectId.ToLower().LastOrDefault() == 'a')
                    return "مراجعة عامة على " + subjectName;
                else
                    return "Review on " + subjectName;
            default:
                return string.Empty;
        }
    }
}