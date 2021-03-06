using SharedModule.DTO;
using TeacherDomain.Features.Tracker.CQRS.Query;
using TeacherDomain.Features.Tracker.DTO.Query;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Tracker.CQRS.Query;

public class GetStudentClassActivityQueryHandler : IRequestHandler<GetStudentClassActivityQuery, ICommitResults<StudentClassActivityResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly StudentClient _studentClient;
    public GetStudentClassActivityQueryHandler(TeacherDbContext dbContext, StudentClient studentClient)
    {
        _dbContext = dbContext;
        _studentClient = studentClient;
    }
    public async Task<ICommitResults<StudentClassActivityResponse>> Handle(GetStudentClassActivityQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<TeacherAssignmentActivityTracker> teacherAssignments = await _dbContext.Set<TeacherAssignmentActivityTracker>()
            .Where(a => a.StudentId == request.StudentId && a.ActivityStatus == ActivityStatus.Finished && a.ClassId == request.ClassId)
            .Include(a => a.TeacherAssignmentFK)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync(cancellationToken);

        IEnumerable<TeacherQuizActivityTracker> teacherQuizzes = await _dbContext.Set<TeacherQuizActivityTracker>()
            .Where(a => a.StudentId == request.StudentId && a.ActivityStatus == ActivityStatus.Finished && a.ClassId == request.ClassId)
            .Include(a => a.TeacherQuizFK)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync(cancellationToken);


        ICommitResults<StudentQuizResultResponse>? studentQuizResults = await _studentClient.GetQuizzesResultAsync(request.StudentId, teacherQuizzes.Select(a => a.TeacherQuizFK.QuizId), cancellationToken);

        if (!studentQuizResults.IsSuccess)
        {
            return studentQuizResults.ResultType.GetValueCommitResults(Array.Empty<StudentClassActivityResponse>(), studentQuizResults.ErrorCode, studentQuizResults.ErrorMessage);
        }

        IEnumerable<StudentClassActivityResponse> Mapper()
        {
            foreach (TeacherQuizActivityTracker quizTracker in teacherQuizzes)
            {
                StudentQuizResultResponse response = studentQuizResults.Value.FirstOrDefault(a => a.QuizId == quizTracker.TeacherQuizFK.QuizId);

                yield return new StudentClassActivityResponse
                {
                    ActivityType = 1,
                    EndDate = quizTracker.CreatedOn.GetValueOrDefault(),
                    StartDate = quizTracker.TeacherQuizFK.StartDate,
                    Title = quizTracker.TeacherQuizFK.Title,
                    TotalScore = response.QuizScore,
                    StudentScore = response.StudentScore
                };
            }
            foreach (TeacherAssignmentActivityTracker assingmnetTracker in teacherAssignments)
            {
                yield return new StudentClassActivityResponse
                {
                    ActivityType = 2,
                    EndDate = assingmnetTracker.CreatedOn.GetValueOrDefault(),
                    StartDate = assingmnetTracker.TeacherAssignmentFK.CreatedOn.GetValueOrDefault(),
                    Title = assingmnetTracker.TeacherAssignmentFK.Title,
                };
            }
            yield break;
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
