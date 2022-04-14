using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Tracker.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Command
{
    public class UpdateStudentQuizCommandHandler : IRequestHandler<UpdateStudentQuizCommand, CommitResult>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly Guid? _userId;

        public UpdateStudentQuizCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<CommitResult> Handle(UpdateStudentQuizCommand request, CancellationToken cancellationToken)
        {
            StudentQuizTracker? quizTracker = await _dbContext.Set<StudentQuizTracker>()
                                                  .SingleOrDefaultAsync(a => a.QuizId.Equals(request.UpdateStudentQuizRequest.QuizId) && a.StudentUserId.Equals(_userId), cancellationToken);
            if (quizTracker == null)
            {
                _dbContext.Set<StudentQuizTracker>().Add(new StudentQuizTracker
                {
                    StudentUserId = _userId.GetValueOrDefault(),
                    StudentUserScore = request.UpdateStudentQuizRequest.StudentUserScore,
                    TimeSpentInSec = request.UpdateStudentQuizRequest.TimeSpentInSec,
                    TotalQuizScore = request.UpdateStudentQuizRequest.TotalQuizScore,
                    QuizId = request.UpdateStudentQuizRequest.QuizId,
                });
            }
            else
            {
                quizTracker.StudentUserScore = request.UpdateStudentQuizRequest.StudentUserScore;
                quizTracker.TimeSpentInSec = request.UpdateStudentQuizRequest.TimeSpentInSec;
                _dbContext.Set<StudentQuizTracker>().Update(quizTracker);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}
