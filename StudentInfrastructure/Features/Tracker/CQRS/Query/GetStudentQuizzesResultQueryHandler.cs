using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetStudentQuizzesResultQueryHandler : IRequestHandler<GetStudentQuizzesResultQuery, ICommitResults<StudentQuizResultResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetStudentQuizzesResultQueryHandler(StudentDbContext dbContext,
                                               IWebHostEnvironment configuration,
                                               IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }
    public async Task<ICommitResults<StudentQuizResultResponse>> Handle(GetStudentQuizzesResultQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<QuizTracker> studentQuizTrackers = await _dbContext.Set<QuizTracker>()
                                                                       .Where(a => request.StudentQuizResultRequest.QuizIds.Contains(a.QuizId) &&
                                                                              a.StudentUserId.Equals(request.StudentQuizResultRequest.StudentId))
                                                                       .ToListAsync(cancellationToken);

        IEnumerable<StudentQuizResultResponse> Mapper()
        {
            foreach (QuizTracker tracker in studentQuizTrackers)
            {
                yield return new StudentQuizResultResponse
                {
                    QuizId = tracker.QuizId,
                    QuizScore = tracker.TotalQuizScore,
                    StudentScore = tracker.StudentUserScore,
                    StudentId = tracker.StudentUserId
                };
            }
            yield break;
        }
        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
