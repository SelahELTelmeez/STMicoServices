using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetActivitiesByClassQueryHandler : IRequestHandler<GetActivitiesByClassQuery, ICommitResults<ClassActivityResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetActivitiesByClassQueryHandler(TeacherDbContext dbContext,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }
    public async Task<ICommitResults<ClassActivityResponse>> Handle(GetActivitiesByClassQuery request, CancellationToken cancellationToken)
    {
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
            .Where(a => a.IsActive && a.Id.Equals(request.ClassId))
            .Include(a => a.TeacherAssignments)
            .Include(a => a.TeacherQuizs)
            .SingleOrDefaultAsync(cancellationToken);

        if (teacherClass == null)
        {
            return ResultType.NotFound.GetValueCommitResults(Array.Empty<ClassActivityResponse>(), "X0001", _resourceJsonManager["X0001"]);
        }

        IEnumerable<TeacherAssignmentActivityTracker> assignmentActivityTrackers = await _dbContext.Set<TeacherAssignmentActivityTracker>()
            .Where(a => a.ActivityStatus == ActivityStatus.Finished && a.ClassId.Equals(request.ClassId))
            .ToListAsync(cancellationToken);


        IEnumerable<TeacherQuizActivityTracker> quizActivityTrackers = await _dbContext.Set<TeacherQuizActivityTracker>()
            .Where(a => a.ActivityStatus == ActivityStatus.Finished && a.ClassId.Equals(request.ClassId))
            .ToListAsync(cancellationToken);


        IEnumerable<ClassActivityResponse> Mapper()
        {
            foreach (TeacherQuiz teacherQuiz in teacherClass.TeacherQuizs)
            {
                yield return new ClassActivityResponse
                {
                    EndDate = teacherQuiz.EndDate,
                    Id = teacherQuiz.Id,
                    StartDate = teacherQuiz.CreatedOn.GetValueOrDefault(),
                    Title = teacherQuiz.Title,
                    TypeValue = 1,
                    TypeName = "Quiz",
                    EnrolledCounter = quizActivityTrackers.Where(a => a.TeacherQuizId.Equals(teacherQuiz.Id)).Count()
                };
            }
            foreach (TeacherAssignment teacherAssignment in teacherClass.TeacherAssignments)
            {
                yield return new ClassActivityResponse
                {
                    EndDate = teacherAssignment.EndDate,
                    Id = teacherAssignment.Id,
                    StartDate = teacherAssignment.CreatedOn.GetValueOrDefault(),
                    Title = teacherAssignment.Title,
                    TypeValue = 2,
                    TypeName = "Assignment",
                    EnrolledCounter = assignmentActivityTrackers.Where(a => a.TeacherAssignmentId.Equals(teacherAssignment.Id)).Count()
                };
            }
            yield break;
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}