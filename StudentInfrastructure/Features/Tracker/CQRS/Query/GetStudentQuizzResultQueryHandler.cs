using SharedModule.DTO;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetStudentQuizzResultQueryHandler : IRequestHandler<GetStudentQuizzResultQuery, ICommitResult<StudentQuizResultResponse>>
    {
        private readonly StudentDbContext _dbContext;
        public GetStudentQuizzResultQueryHandler(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICommitResult<StudentQuizResultResponse>> Handle(GetStudentQuizzResultQuery request, CancellationToken cancellationToken)
        {
            QuizTracker? studentQuizTracker = await _dbContext.Set<QuizTracker>()
                                                              .Where(a => request.QuizId.Equals(a.QuizId) && a.StudentUserId.Equals(request.StudentId))
                                                              .SingleOrDefaultAsync(cancellationToken);

            if (studentQuizTracker == null)
            {
                return ResultType.NotFound.GetValueCommitResult((StudentQuizResultResponse)null, "XXXX", "X000");
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
