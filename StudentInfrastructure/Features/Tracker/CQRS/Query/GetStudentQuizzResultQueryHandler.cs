using SharedModule.DTO;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentEntities.Entities.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetStudentQuizzResultQueryHandler : IRequestHandler<GetStudentQuizzResultQuery, CommitResult<StudentQuizResultResponse>>
    {
        private readonly StudentDbContext _dbContext;
        public GetStudentQuizzResultQueryHandler(StudentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommitResult<StudentQuizResultResponse>> Handle(GetStudentQuizzResultQuery request, CancellationToken cancellationToken)
        {
            QuizTracker? studentQuizTracker = await _dbContext.Set<QuizTracker>()
                                                              .Where(a => request.QuizId.Equals(a.QuizId) && a.StudentUserId.Equals(request.StudentId))
                                                              .SingleOrDefaultAsync(cancellationToken);

            if (studentQuizTracker == null)
            {
                return new CommitResult<StudentQuizResultResponse>
                {
                    ErrorCode = "XXXX",
                    ErrorMessage = "X000",
                    ResultType = ResultType.NotFound
                };
            }
            return new CommitResult<StudentQuizResultResponse>
            {
                ResultType = ResultType.Ok,
                Value = new StudentQuizResultResponse
                {
                    QuizId = request.QuizId,
                    QuizScore = studentQuizTracker.TotalQuizScore,
                    StudentScore = studentQuizTracker.StudentUserScore,
                    StudentId = studentQuizTracker.StudentUserId
                }
            };
        }
    }
}
