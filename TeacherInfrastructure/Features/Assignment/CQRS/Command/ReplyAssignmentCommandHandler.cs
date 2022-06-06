using TeacherDomain.Features.Assignment.CQRS.Command;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.Trackers;

namespace TeacherInfrastructure.Features.Assignment.CQRS.Command
{
    public class ReplyAssignmentCommandHandler : IRequestHandler<ReplyAssignmentCommand, ICommitResult>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly Guid? _userId;
        public ReplyAssignmentCommandHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<ICommitResult> Handle(ReplyAssignmentCommand request, CancellationToken cancellationToken)
        {
            TeacherAssignmentActivityTracker? activityTracker = await _dbContext.Set<TeacherAssignmentActivityTracker>().SingleOrDefaultAsync(a => a.Id.Equals(request.ReplyAssignmentRequest.AssignmentActivityTrackerId) && a.StudentId.Equals(_userId), cancellationToken);

            if (activityTracker == null)
            {
                return ResultType.NotFound.GetCommitResult();
            }

            activityTracker.ActivityStatus = ActivityStatus.Finished;
            activityTracker.ReplyComment = request.ReplyAssignmentRequest.ReplyComment;
            activityTracker.ReplyAttachmentId = request.ReplyAssignmentRequest.ReplyAttachmentId;

            _dbContext.Set<TeacherAssignmentActivityTracker>().Update(activityTracker);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}
