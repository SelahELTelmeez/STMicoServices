using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Assignment.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Assignment.CQRS.Command
{
    public class ReplyAssignmentCommandHandler : IRequestHandler<ReplyAssignmentCommand, CommitResult>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly Guid? _userId;
        public ReplyAssignmentCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<CommitResult> Handle(ReplyAssignmentCommand request, CancellationToken cancellationToken)
        {
            TeacherAssignmentActivityTracker? activityTracker = await _dbContext.Set<TeacherAssignmentActivityTracker>().SingleOrDefaultAsync(a => a.Id.Equals(request.ReplyAssignmentRequest.AssignmentActivityTrackerId) && a.StudentId.Equals(_userId), cancellationToken);

            if (activityTracker == null)
            {
                return new CommitResult
                {
                    ResultType = ResultType.NotFound,
                };
            }

            activityTracker.ActivityStatus = ActivityStatus.Finished;
            activityTracker.ReplyComment = request.ReplyAssignmentRequest.ReplyComment;
            activityTracker.ReplyAttachmentUrl = request.ReplyAssignmentRequest.ReplyAttachmentUrl;

            _dbContext.Set<TeacherAssignmentActivityTracker>().Update(activityTracker);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}
