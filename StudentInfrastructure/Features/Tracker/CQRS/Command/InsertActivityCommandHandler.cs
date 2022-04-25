using Microsoft.EntityFrameworkCore.ChangeTracking;
using StudentDomain.Features.Activities.CQRS.Command;
using StudentEntities.Entities;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.Utilities;

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
        StudentActivityTracker? StudentActivityTrackerChecker = await _dbContext.Set<StudentActivityTracker>()
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
        StudentActivityTracker activityTracker = request.ActivityRequest.Adapt<StudentActivityTracker>();
        activityTracker.StudentId = _userId.GetValueOrDefault();
        EntityEntry<StudentActivityTracker> studentActivityTracker = _dbContext.Set<StudentActivityTracker>().Add(activityTracker);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get Response ActivityId ================
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = studentActivityTracker.Entity.Id
        };
    }
}
