using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetStudentRecentLessonsProgressQueryHandler : IRequestHandler<GetStudentRecentLessonsProgressQuery, ICommitResults<StudentRecentLessonProgressResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly string? _userId;
    private readonly CurriculumClient _CurriculumClient;
    public GetStudentRecentLessonsProgressQueryHandler(CurriculumClient curriculumClient, IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<ICommitResults<StudentRecentLessonProgressResponse>> Handle(GetStudentRecentLessonsProgressQuery request, CancellationToken cancellationToken)
    {
        // Read all User's activity 

        IEnumerable<ActivityTracker> activityTrackers = await _dbContext.Set<ActivityTracker>()
                                                                        .Where(a => a.StudentId.Equals(_userId) && a.IsActive)
                                                                        .GroupBy(a => a.LessonId)
                                                                        .Select(a => a.OrderByDescending(b => b.CreatedOn).First())
                                                                        .ToListAsync(cancellationToken);

        if (activityTrackers.Any())
        {
            List<int> activityRecords = activityTrackers.OrderByDescending(a => a.CreatedOn).Select(a => a.LessonId).Take(2).ToList();

            ICommitResults<LessonBriefResponse>? lessonBreifs = await _CurriculumClient.GetLessonsBriefAsync(activityRecords, cancellationToken);
            if (lessonBreifs.IsSuccess)
            {
                IEnumerable<StudentRecentLessonProgressResponse> Mapper()
                {
                    foreach (LessonBriefResponse briefResponse in lessonBreifs.Value)
                    {
                        yield return new StudentRecentLessonProgressResponse
                        {
                            LessonName = briefResponse.Name,
                            LessonPoints = briefResponse.Points.GetValueOrDefault(),
                            StudentPoints = _dbContext.Set<ActivityTracker>().Where(a => a.LessonId.Equals(briefResponse.Id)).AsEnumerable().DistinctBy(a => a.ClipId).Sum(a => a.StudentPoints)
                        };
                    }
                    yield break;
                }
                return ResultType.Ok.GetValueCommitResults(Mapper());
            }

        }
        return ResultType.Ok.GetValueCommitResults(Array.Empty<StudentRecentLessonProgressResponse>());
    }
}