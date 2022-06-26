using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Reports.CQRS.Query;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Reports.CQRS.Query
{
    public class RecentActivityQueryHandler : IRequestHandler<RecentActivityQuery, ICommitResults<RecentActivityResponse>>
    {
        private readonly StudentDbContext _dbContext;
        private readonly CurriculumClient _curriculumClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public RecentActivityQueryHandler(CurriculumClient curriculumClient,
                                          StudentDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor)
        {
            _curriculumClient = curriculumClient;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResults<RecentActivityResponse>> Handle(RecentActivityQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ActivityTracker> activityTrackers = await _dbContext.Set<ActivityTracker>()
                                                        .Where(a => a.StudentId.Equals(request.StudentId ?? _httpContextAccessor.GetIdentityUserId()))
                                                        .Where(a => a.IsActive)
                                                        .GroupBy(a => a.SubjectId)
                                                        .Select(a => a.OrderByDescending(b => b.CreatedOn).First())
                                                        .ToListAsync(cancellationToken);
            if (!activityTrackers.Any())
            {
                return ResultType.Empty.GetValueCommitResults(Array.Empty<RecentActivityResponse>(), "XSTU0003", _resourceJsonManager["XSTU0003"]);
            }
            ICommitResults<SubjectDetailedResponse>? subjectResult = await _curriculumClient.GetSubjectsDetailedAsync(activityTrackers.Select(a => a.SubjectId), cancellationToken);

            if (!subjectResult.IsSuccess)
            {
                return subjectResult.ResultType.GetValueCommitResults(Array.Empty<RecentActivityResponse>(), subjectResult.ErrorCode, subjectResult.ErrorMessage);
            }

            IEnumerable<SubjectDetailedResponse> filteredSubjects = subjectResult.Value.Where(a => a.Term == request.Term).DistinctBy(a => a.Id).ToList();

            IEnumerable<ClipSubjectBreifResponse> allClips = filteredSubjects.SelectMany(a => a.UnitResponses).SelectMany(a => a.Lessons).SelectMany(a => a.Clips);
            IEnumerable<LessonSubjectBriefResponse> allLessons = filteredSubjects.SelectMany(a => a.UnitResponses).SelectMany(a => a.Lessons);
            IEnumerable<UnitSubjectBriefResponse> allUnits = filteredSubjects.SelectMany(a => a.UnitResponses);


            IEnumerable<RecentActivityResponse> Mapper()
            {
                foreach (SubjectDetailedResponse item in filteredSubjects)
                {
                    ActivityTracker activityTracker = activityTrackers.SingleOrDefault(a => a.SubjectId == item.Id);

                    ClipSubjectBreifResponse? clipSubjectBreifResponse = allClips.FirstOrDefault(a => a.ClipId == activityTracker.ClipId);
                    LessonSubjectBriefResponse? lessonSubjectBriefResponse = allLessons.FirstOrDefault(a => a.LessonId == activityTracker.LessonId);
                    UnitSubjectBriefResponse? unitSubjectBriefResponse = allUnits.Where(a => a.Lessons.FirstOrDefault(a => a.LessonId == lessonSubjectBriefResponse.LessonId) != null)?.FirstOrDefault();

                    yield return new RecentActivityResponse
                    {
                        SubjectId = item.Id,
                        SubjectName = item.ShortName,
                        ActivityTime = activityTracker.CreatedOn.GetValueOrDefault(),
                        ClipName = clipSubjectBreifResponse?.ClipName,
                        UnitName = unitSubjectBriefResponse.UnitName,
                        LessonName = lessonSubjectBriefResponse?.LessonName,
                        SubjectImage = item.InternalIcon
                    };
                }
                yield break;
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}
