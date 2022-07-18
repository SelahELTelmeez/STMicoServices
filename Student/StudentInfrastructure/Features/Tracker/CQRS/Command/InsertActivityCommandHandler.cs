using Microsoft.EntityFrameworkCore.ChangeTracking;
using SharedModule.Extensions;
using StudentDomain.Features.Activities.CQRS.Command;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Activities.CQRS.Command;
public class InsertActivityCommandHandler : IRequestHandler<InsertActivityCommand, ICommitResult<int>>
{
    private readonly StudentDbContext _dbContext;
    private readonly string? _userId;
    private readonly IdentityClient _identityClient;

    public InsertActivityCommandHandler(StudentDbContext dbContext,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor,
                                        IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }

    public async Task<ICommitResult<int>> Handle(InsertActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== Check for the clip of this student existance first ================
        ActivityTracker? StudentActivityTrackerChecker = await _dbContext.Set<ActivityTracker>()
                                                                         .FirstOrDefaultAsync(a => a.StudentId.Equals(_userId) && a.ClipId.Equals(request.ActivityRequest.ClipId), cancellationToken);
        if (StudentActivityTrackerChecker != null)
        {
            return ResultType.Ok.GetValueCommitResult(StudentActivityTrackerChecker.Id);
        }


        ICommitResult<int>? currentStudentGrade = await _identityClient.GetStudentGradeAsync(_userId, cancellationToken);


        if (!currentStudentGrade.IsSuccess)
        {
            currentStudentGrade.ResultType.GetValueCommitResult(0, currentStudentGrade.ErrorCode, currentStudentGrade.ErrorMessage);
        }

        // =========== insert student Activity ================
        ActivityTracker activityTracker = request.ActivityRequest.Adapt<ActivityTracker>();
        activityTracker.StudentId = _userId;
        activityTracker.IsActive = true;
        activityTracker.GradeId = currentStudentGrade.Value;
        EntityEntry<ActivityTracker> studentActivityTracker = _dbContext.Set<ActivityTracker>().Add(activityTracker);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get Response Id ================
        return ResultType.Ok.GetValueCommitResult(studentActivityTracker.Entity.Id);
    }
}
