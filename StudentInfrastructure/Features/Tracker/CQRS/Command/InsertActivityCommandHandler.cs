using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedModule.Extensions;
using StudentDomain.Features.Activities.CQRS.Command;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Activities.CQRS.Command;
public class InsertActivityCommandHandler : IRequestHandler<InsertActivityCommand, CommitResult<int>>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IdentityClient _identityClient;

    public InsertActivityCommandHandler(StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
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


        CommitResult<int>? currentStudentGrade = await _identityClient.GetStudentGradeAsync(_userId, cancellationToken);


        if (!currentStudentGrade.IsSuccess)
        {
            return new CommitResult<int>
            {
                ErrorCode = currentStudentGrade.ErrorCode,
                ErrorMessage = currentStudentGrade.ErrorMessage,
                ResultType = currentStudentGrade.ResultType
            };
        }

        // =========== insert student Activity ================
        ActivityTracker activityTracker = request.ActivityRequest.Adapt<ActivityTracker>();
        activityTracker.StudentId = _userId.GetValueOrDefault();
        activityTracker.IsActive = true;
        activityTracker.GradeId = currentStudentGrade.Value;
        EntityEntry<ActivityTracker> studentActivityTracker = _dbContext.Set<ActivityTracker>().Add(activityTracker);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get Response Id ================
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = studentActivityTracker.Entity.Id
        };
    }
}
