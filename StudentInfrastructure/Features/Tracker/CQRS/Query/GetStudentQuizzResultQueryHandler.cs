using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetStudentQuizzResultQueryHandler : IRequestHandler<GetStudentQuizzResultQuery, ICommitResult<StudentQuizResultResponse>>
    {
        private readonly StudentDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetStudentQuizzResultQueryHandler(StudentDbContext dbContext,
                                                 IWebHostEnvironment configuration,
                                                 IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResult<StudentQuizResultResponse>> Handle(GetStudentQuizzResultQuery request, CancellationToken cancellationToken)
        {
            QuizTracker? studentQuizTracker = await _dbContext.Set<QuizTracker>()
                                                              .Where(a => request.QuizId.Equals(a.QuizId) && a.StudentUserId.Equals(request.StudentId))
                                                              .SingleOrDefaultAsync(cancellationToken);

            if (studentQuizTracker == null)
            {
                return ResultType.NotFound.GetValueCommitResult<StudentQuizResultResponse>(default, "X0004", _resourceJsonManager["X0004"]);
            }
            return ResultType.Ok.GetValueCommitResult(new StudentQuizResultResponse
            {
                QuizId = request.QuizId,
                QuizScore = studentQuizTracker.TotalQuizScore,
                StudentScore = studentQuizTracker.StudentUserScore,
                StudentId = studentQuizTracker.StudentUserId
            });
        }
    }
}
