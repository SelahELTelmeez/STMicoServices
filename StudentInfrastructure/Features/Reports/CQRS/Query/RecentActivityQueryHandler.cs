using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Reports.CQRS.Query;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Reports.CQRS.Query
{
    public class RecentActivityQueryHandler : IRequestHandler<RecentActivityQuery, CommitResults<RecentActivityResponse>>
    {
        private readonly StudentDbContext _dbContext;
        private readonly CurriculumClient _curriculumClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecentActivityQueryHandler(CurriculumClient curriculumClient, StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _curriculumClient = curriculumClient;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommitResults<RecentActivityResponse>> Handle(RecentActivityQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ActivityTracker> activityTrackers = await _dbContext.Set<ActivityTracker>()
                                                        .Where(a => a.StudentId.Equals(request.StudentId ?? _httpContextAccessor.GetIdentityUserId()))
                                                        .GroupBy(a => a.SubjectId)
                                                        .Select(a => a.OrderByDescending(b => b.CreatedOn).First())
                                                        .ToListAsync(cancellationToken);

            CommitResults<SubjectDetailedResponse>? subjectResult = await _curriculumClient.GetSubjectsDetailedAsync(activityTrackers.Select(a => a.SubjectId), cancellationToken);

            if (!subjectResult.IsSuccess)
            {
                return subjectResult.Adapt<CommitResults<RecentActivityResponse>>();
            }

            IEnumerable<SubjectDetailedResponse> filteredSubjects = subjectResult.Value.Where(a => a.Term == request.Term).DistinctBy(a => a.Id).ToList();

            IEnumerable<ClipSubjectBreifResponse> allClips = filteredSubjects.SelectMany(a => a.UnitResponses).SelectMany(a => a.Lessons).SelectMany(a => a.Clips);
            IEnumerable<LessonSubjectBriefResponse> allLessons = filteredSubjects.SelectMany(a => a.UnitResponses).SelectMany(a => a.Lessons);


            IEnumerable<RecentActivityResponse> Mapper()
            {
                foreach (SubjectDetailedResponse item in filteredSubjects)
                {
                    ActivityTracker activityTracker = activityTrackers.SingleOrDefault(a => a.SubjectId == item.Id);


                    yield return new RecentActivityResponse
                    {
                        SubjectId = item.Id,
                        SubjectName = item.ShortName,
                        ActivityTime = activityTracker.CreatedOn.GetValueOrDefault(),
                        ClipName = allClips.FirstOrDefault(a => a.ClipId == activityTracker.ClipId)?.ClipName,
                        LessonName = allLessons.FirstOrDefault(a => a.LessonId == activityTracker.LessonId)?.LessonName,
                        SubjectImage = item.InternalIcon
                    };
                }
                yield break;
            }

            return new CommitResults<RecentActivityResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
