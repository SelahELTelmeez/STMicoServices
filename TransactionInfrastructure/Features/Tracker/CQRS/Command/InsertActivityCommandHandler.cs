using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using TransactionDomain.Features.Activities.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Activities.CQRS.Command;

public class InsertActivityCommandHandler : IRequestHandler<InsertActivityCommand, CommitResult<int>>
{
    private readonly StudentTrackerDbContext _dbContext;
    private readonly Guid? _userId;

    // private readonly JsonLocalizerManager _resourceJsonManager;
    //private readonly TokenHandlerManager _jwtAccessGenerator;
    //private readonly INotificationService _notificationService;

    public InsertActivityCommandHandler(StudentTrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        //_resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        //_jwtAccessGenerator = tokenHandlerManager;
        //_notificationService = notificationService;
    }

    public async Task<CommitResult<int>> Handle(InsertActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== insert student Activity ================
        StudentActivityTracker studentActivity = new StudentActivityTracker
        {
            StudentId = _userId.GetValueOrDefault(),
            StudentPoints = request.ActivityRequest.StudentPoints,
            LearningDurationInSec = request.ActivityRequest.LearningDurationInSec,
            Code = request.ActivityRequest.Code,
            Progress = request.ActivityRequest.Progress,
            ClipId = request.ActivityRequest.ClipId
        };

        _dbContext.Set<StudentActivityTracker>().Add(studentActivity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== check Lesson Activity ================

        // =========== Check for the lesson existance first ================
        StudentLessonTracker? StudentLesson = await _dbContext.Set<StudentLessonTracker>().FirstOrDefaultAsync(a => a.StudentId.Equals(_userId)
                                                                                         && a.LessonId.Equals(request.ActivityRequest.LessonId), cancellationToken);
        // =========== Update Lesson Activity ================
        if (StudentLesson != null)
        {
            StudentLesson.StudentPoints = request.ActivityRequest.StudentPoints;
            StudentLesson.LastDateTime = DateTime.UtcNow;
            _dbContext.Set<StudentLessonTracker>().Update(StudentLesson);
        }
        // =========== insert Lesson Activity ================
        else
        {
            StudentLesson = new StudentLessonTracker
            {
                StudentId = _userId.GetValueOrDefault(),
                LessonId = request.ActivityRequest.LessonId,
                StudentPoints = request.ActivityRequest.StudentPoints,
                LastDateTime = DateTime.UtcNow
            };
            _dbContext.Set<StudentLessonTracker>().Add(StudentLesson);
        }
        // ===========save changes and return================
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get Response ActivityId ================

        return new CommitResult<int>
        {
            ResultType = ResultType.Ok, // : ResultType.PartialOk,
            Value = studentActivity.Id
        };
    }
}
