using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentInfrastructure.HttpClients;
using DomainEntities = StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetClipActivityQueryHandler : IRequestHandler<GetClipActivityQuery, ICommitResults<ClipActivityResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly string? _userId;
    public GetClipActivityQueryHandler(IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }
    public async Task<ICommitResults<ClipActivityResponse>> Handle(GetClipActivityQuery request, CancellationToken cancellationToken)
    {
        ICommitResult<int>? currentStudentGrade = await _identityClient.GetStudentGradeAsync(_userId, cancellationToken);

        if (!currentStudentGrade.IsSuccess)
        {
            currentStudentGrade.ResultType.GetValueCommitResults(Array.Empty<ClipActivityResponse>(), currentStudentGrade.ErrorCode, currentStudentGrade.ErrorMessage);
        }

        IEnumerable<DomainEntities.ActivityTracker> activityTrackers = await _dbContext.Set<DomainEntities.ActivityTracker>()
                                    .Where(a => request.ClipIds.Contains(a.ClipId) && a.StudentId.Equals(_userId) && a.GradeId == currentStudentGrade.Value)
                                    .ToListAsync(cancellationToken);

        IEnumerable<DomainEntities.QuizTracker> quizTrackers = await _dbContext.Set<DomainEntities.QuizTracker>()
                                    .Where(a => request.ClipIds.Contains(a.ClipId.GetValueOrDefault()) && a.StudentId.Equals(_userId))
                                    .ToListAsync(cancellationToken);


        IEnumerable<ClipActivityResponse> Mapper()
        {
            foreach (DomainEntities.ActivityTracker tracker in activityTrackers.Where(a => !quizTrackers.Select(a => a.ClipId).Contains(a.ClipId)))
            {
                yield return new ClipActivityResponse
                {
                    ClipId = tracker.ClipId,
                    GameObjectCode = tracker.Code,
                    GameObjectLearningDurationInSec = tracker.LearningDurationInSec,
                    GameObjectProgress = tracker.Progress,
                    Id = tracker.Id,
                    LearningObjectAsJson = tracker.LearningObjectAsJson,
                    StudentPoints = tracker.StudentPoints
                };
            }
            foreach (DomainEntities.QuizTracker tracker in quizTrackers)
            {
                yield return new ClipActivityResponse
                {
                    ClipId = tracker.ClipId.GetValueOrDefault(),
                    GameObjectCode = default,
                    GameObjectLearningDurationInSec = tracker.TimeSpentInSec,
                    GameObjectProgress = default,
                    Id = tracker.Id,
                    LearningObjectAsJson = default,
                    StudentPoints = tracker.StudentUserScore,
                    QuizScore = tracker.TotalQuizScore
                };
            }
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
