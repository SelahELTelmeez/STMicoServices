using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedModule.Extensions;
using StudentDomain.Features.Activities.CQRS.Command;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Activities.CQRS.Command;
public class InsertActivityCommandHandler : IRequestHandler<InsertActivityCommand, CommitResult<int>>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;

    public InsertActivityCommandHandler(StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult<int>> Handle(InsertActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== Check for the clip of this student existance first ================
        ActivityTracker? StudentActivityTrackerChecker = await _dbContext.Set<ActivityTracker>()
                                                                                .SingleOrDefaultAsync(a => a.StudentId.Equals(_userId) && a.ClipId.Equals(request.ActivityRequest.ClipId), cancellationToken);
        if (StudentActivityTrackerChecker != null)
        {
            return new CommitResult<int>
            {
                ResultType = ResultType.Ok,
                Value = StudentActivityTrackerChecker.Id
            };
        }
        // =========== insert student Activity ================
        ActivityTracker activityTracker = request.ActivityRequest.Adapt<ActivityTracker>();
        activityTracker.StudentId = _userId.GetValueOrDefault();
        EntityEntry<ActivityTracker> studentActivityTracker = _dbContext.Set<ActivityTracker>().Add(activityTracker);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get Response ActivityId ================
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = studentActivityTracker.Entity.Id
        };
    }
}
