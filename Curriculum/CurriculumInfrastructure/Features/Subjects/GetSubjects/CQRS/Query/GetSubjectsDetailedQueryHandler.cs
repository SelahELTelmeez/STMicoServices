using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Subjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query
{
    public class GetSubjectsDetailedQueryHandler : IRequestHandler<GetSubjectsDetailedQuery, CommitResults<SubjectDetailedResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IDistributedCache _cache;

        public GetSubjectsDetailedQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }


        public async Task<CommitResults<SubjectDetailedResponse>> Handle(GetSubjectsDetailedQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<SubjectDetailedResponse>? cachedUnitSubjectBriefResponse = await _cache.GetFromCacheAsync<string, IEnumerable<SubjectDetailedResponse>>(string.Join(',', request.SubjectIds), "Curriculum-GetSubjectsDetailed", cancellationToken);

            if (cachedUnitSubjectBriefResponse == null)
            {
                IEnumerable<Subject> subjects = await _dbContext.Set<Subject>()
                                                           .Where(a => request.SubjectIds.Contains(a.Id))
                                                           .Include(a => a.Units)
                                                           .ThenInclude(a => a.Lessons)
                                                           .ThenInclude(a => a.Clips)
                                                           .ToListAsync(cancellationToken);

                if (subjects.Any())
                {
                    IEnumerable<UnitSubjectBriefResponse> UnitMapper(IEnumerable<CurriculumEntites.Entities.Units.Unit> units)
                    {
                        foreach (CurriculumEntites.Entities.Units.Unit unit in units)
                        {
                            yield return new UnitSubjectBriefResponse
                            {
                                UnitId = unit.Id,
                                UnitName = unit.ShortName,
                                Lessons = LessonMapper(unit.Lessons)
                            };
                        }
                        yield break;
                    }

                    IEnumerable<LessonSubjectBriefResponse> LessonMapper(IEnumerable<Lesson> lessons)
                    {
                        foreach (Lesson lesson in lessons)
                        {
                            yield return new LessonSubjectBriefResponse
                            {
                                LessonId = lesson.Id,
                                LessonName = lesson.ShortName,
                                Clips = ClipMapper(lesson.Clips)
                            };
                        }
                        yield break;

                    }
                    IEnumerable<ClipSubjectBreifResponse> ClipMapper(IEnumerable<Clip> clips)
                    {
                        foreach (Clip clip in clips)
                        {
                            yield return new ClipSubjectBreifResponse
                            {
                                ClipId = clip.Id,
                                ClipName = clip.Title,
                            };
                        }
                        yield break;
                    }

                    IEnumerable<SubjectDetailedResponse> Mapper()
                    {
                        foreach (Subject subject in subjects)
                        {
                            yield return new SubjectDetailedResponse
                            {
                                FullyQualifiedName = subject.FullyQualifiedName,
                                Grade = subject.Grade,
                                Id = subject.Id,
                                IsAppShow = subject.IsAppShow,
                                RewardPoints = subject.RewardPoints,
                                ShortName = subject.ShortName,
                                TeacherGuide = subject.TeacherGuide,
                                Term = subject.Term,
                                Title = subject.Title,
                                PrimaryIcon = $"http://www.almoallem.com/media/LMSAPP/TeacherSubjectIcon/{subject.Id[..6]}.png",
                                InternalIcon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png",
                                UnitResponses = UnitMapper(subject.Units),
                            };
                        }
                        yield break;
                    }
                    cachedUnitSubjectBriefResponse = Mapper();

                    await _cache.SaveToCacheAsync(string.Join(',', request.SubjectIds), cachedUnitSubjectBriefResponse, "Curriculum-GetSubjectsDetailed", cancellationToken);
                }
                else
                {
                    return new CommitResults<SubjectDetailedResponse>()
                    {
                        ResultType = ResultType.Empty,
                        Value = Array.Empty<SubjectDetailedResponse>()
                    };
                }
            }


            return new CommitResults<SubjectDetailedResponse>
            {
                ResultType = ResultType.Ok,
                Value = cachedUnitSubjectBriefResponse
            };

        }
    }
}
