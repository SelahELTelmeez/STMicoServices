using SharedModule.DTO;
using TeacherDomain.Features.Tracker.CQRS.Query;
using TeacherDomain.Features.Tracker.DTO.Query;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetDetailedStudentClassActivitiesQueryHandler : IRequestHandler<GetDetailedStudentClassActivitiesQuery, ICommitResults<DetailedClassActivity>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly StudentClient _studentClient;
        private readonly Guid? _studentId;
        public GetDetailedStudentClassActivitiesQueryHandler(TeacherDbContext dbContext, StudentClient studentClient, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _studentClient = studentClient;
            _studentId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<ICommitResults<DetailedClassActivity>> Handle(GetDetailedStudentClassActivitiesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherAssignmentActivityTracker> teacherAssignments = await _dbContext.Set<TeacherAssignmentActivityTracker>()
                .Where(a => a.StudentId == _studentId && a.ClassId == request.ClassId)
                .Include(a => a.TeacherAssignmentFK)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync(cancellationToken);

            IEnumerable<TeacherQuizActivityTracker> teacherQuizzes = await _dbContext.Set<TeacherQuizActivityTracker>()
                .Where(a => a.StudentId == _studentId && a.ClassId == request.ClassId)
                .Include(a => a.TeacherQuizFK)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync(cancellationToken);


            ICommitResults<StudentQuizResultResponse>? studentQuizResults = await _studentClient.GetQuizzesResultAsync(_studentId.GetValueOrDefault(), teacherQuizzes.Select(a => a.TeacherQuizFK.QuizId), cancellationToken);

            if (!studentQuizResults.IsSuccess)
            {
                return studentQuizResults.ResultType.GetValueCommitResults(Array.Empty<DetailedClassActivity>(), studentQuizResults.ErrorCode, studentQuizResults.ErrorMessage);
            }

            IEnumerable<DetailedClassActivity> Mapper()
            {
                foreach (TeacherQuizActivityTracker quizTracker in teacherQuizzes)
                {
                    StudentQuizResultResponse? response = studentQuizResults.Value.SingleOrDefault(a => a.QuizId == quizTracker.TeacherQuizFK.QuizId);

                    yield return new DetailedClassActivity
                    {
                        Id = quizTracker.TeacherQuizId,
                        IsFinished = quizTracker.ActivityStatus == ActivityStatus.Finished,
                        ActivityType = 1,
                        EndDate = quizTracker.CreatedOn.GetValueOrDefault(),
                        StartDate = quizTracker.TeacherQuizFK.StartDate,
                        Title = quizTracker.TeacherQuizFK.Title,
                        Description = quizTracker.TeacherQuizFK.Description,
                        QuizScore = response?.QuizScore,
                        StudentScore = response?.StudentScore
                    };
                }
                foreach (TeacherAssignmentActivityTracker assingmnetTracker in teacherAssignments)
                {
                    yield return new DetailedClassActivity
                    {
                        Id = assingmnetTracker.TeacherAssignmentId,
                        IsFinished = assingmnetTracker.ActivityStatus == ActivityStatus.Finished,
                        ActivityType = 2,
                        Description = assingmnetTracker.TeacherAssignmentFK.Description,
                        Title = assingmnetTracker.TeacherAssignmentFK.Title,
                        EndDate = assingmnetTracker.CreatedOn.GetValueOrDefault(),
                        StartDate = assingmnetTracker.TeacherAssignmentFK.CreatedOn.GetValueOrDefault(),
                    };
                }
                yield break;
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}