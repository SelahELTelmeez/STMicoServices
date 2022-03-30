using MediatR;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using TransactionDomain.Features.Activities.CQRS.Command;
using TransactionDomain.Features.Activities.DTO.Command;
using TransactionEntites.Entities;
//using JsonLocalizer;
using TransactionEntites.Entities.Activities;
using TransactionEntites.Entities.Lessons;

namespace TransactionInfrastructure.Features.Activities.CQRS.Command
{
    public class ActivityTrackerCommandHandler : IRequestHandler<ActivityCommand, CommitResult<ActivityResponseDTO>>
    {
        private readonly StudentTrackerDbContext _dbContext;
        // private readonly JsonLocalizerManager _resourceJsonManager;
        //private readonly TokenHandlerManager _jwtAccessGenerator;
        //private readonly INotificationService _notificationService;

        public ActivityTrackerCommandHandler(StudentTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
            //_resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            //_jwtAccessGenerator = tokenHandlerManager;
            //_notificationService = notificationService;
        }

        public async Task<CommitResult<ActivityResponseDTO>> Handle(ActivityCommand request, CancellationToken cancellationToken)
        {
            // =========== insert student Activity ================
            StudentActivityTracker studentActivity = new StudentActivityTracker();
            studentActivity.StudentId = request.ActivityRequest.StudentId;
            studentActivity.StudentPoints = request.ActivityRequest.StudentPoints;
            studentActivity.LearningDurationInSec = request.ActivityRequest.LearningDurationInSec;
            studentActivity.Code = request.ActivityRequest.Code;
            studentActivity.progress = request.ActivityRequest.progress;
            studentActivity.ClipId = request.ActivityRequest.ClipId;

            _dbContext.Set<StudentActivityTracker>().Add(studentActivity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // =========== Get Response ActivityId ================
            ActivityResponseDTO responseDTO = new ActivityResponseDTO();
            responseDTO.ActivityId = studentActivity.Id;

            // =========== check Lesson Activity ================

            // =========== Check for the lesson existance first ================
            StudentLessonTracker? StudentLesson = await _dbContext.Set<StudentLessonTracker>().FirstOrDefaultAsync(x => x.StudentId == request.ActivityRequest.StudentId
                                                                                             && x.LessonId == request.ActivityRequest.LessonId, cancellationToken);
            // =========== Update Lesson Activity ================
            if (StudentLesson != null)
            {
                StudentLesson.StudentPoints = request.ActivityRequest.StudentPoints;
                StudentLesson.LastDateTime = DateTime.Now;
            }
            // =========== insert Lesson Activity ================
            else
            {
                StudentLesson = new StudentLessonTracker();
                StudentLesson.StudentId = request.ActivityRequest.StudentId;
                StudentLesson.LessonId = request.ActivityRequest.LessonId;
                StudentLesson.StudentPoints = request.ActivityRequest.StudentPoints;
                StudentLesson.LastDateTime = DateTime.Now;
                _dbContext.Set<StudentLessonTracker>().Add(StudentLesson);
            }
            // ===========save changes and return================
            int x = await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult<ActivityResponseDTO>
            {
                ResultType = ResultType.Ok, // : ResultType.PartialOk,
                Value = responseDTO
            };
        }
    }
}
