using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Quiz.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Quiz.CQRS
{
    public class ReplyQuizCommandHandler : IRequestHandler<ReplyQuizCommand, CommitResult>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly Guid? _userId;
        private readonly CurriculumClient _curriculumClient;
        public ReplyQuizCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _curriculumClient = curriculumClient;
        }
        public async Task<CommitResult> Handle(ReplyQuizCommand request, CancellationToken cancellationToken)
        {
            CommitResult? submitResult = await _curriculumClient.SubmitQuizeAsync(request.ReplyQuizRequest.StudentAnswers, cancellationToken);
            if (!submitResult.IsSuccess)
            {
                return submitResult;
            }
            TeacherQuizActivityTracker? teacherQuizActivityTracker = await _dbContext.Set<TeacherQuizActivityTracker>().SingleOrDefaultAsync(a => a.Id.Equals(request.ReplyQuizRequest.QuizActivityTrackerId), cancellationToken);
            if (teacherQuizActivityTracker == null)
            {
                return new CommitResult
                {
                    ResultType = ResultType.NotFound,
                };
            }
            teacherQuizActivityTracker.ActivityStatus = ActivityStatus.Finished;

            _dbContext.Set<TeacherQuizActivityTracker>().Update(teacherQuizActivityTracker);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}
