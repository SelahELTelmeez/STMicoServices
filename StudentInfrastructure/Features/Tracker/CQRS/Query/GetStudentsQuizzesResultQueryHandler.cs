using SharedModule.DTO;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetStudentsQuizzesResultQueryHandler : IRequestHandler<GetStudentsQuizzesResultQuery, ICommitResults<StudentQuizResultResponse>>
    {
        private readonly StudentDbContext _dbContext;
        public GetStudentsQuizzesResultQueryHandler(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICommitResults<StudentQuizResultResponse>> Handle(GetStudentsQuizzesResultQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<QuizTracker> studentQuizTrackers = await _dbContext.Set<QuizTracker>()
                                                                           .Where(a => request.StudentsQuizResultRequest.QuizIds.Contains(a.QuizId) && request.StudentsQuizResultRequest.StudentIds.Contains(a.StudentUserId))
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
}
