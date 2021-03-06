using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeacherDomain.Features.Quiz.CQRS.Command;
using TeacherEntites.Entities.Shared;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Command;
public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly string? _userId;
    private readonly CurriculumClient _curriculumClient;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateQuizCommandHandler(TeacherDbContext dbContext,
                                    IHttpContextAccessor httpContextAccessor,
                                    IWebHostEnvironment configuration,
                                    CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }
    public async Task<ICommitResult> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {

        ICommitResult<int>? quizResult = await _curriculumClient.CreateQuizeAsync(request.CreateQuizRequest.ClipId, cancellationToken);

        if (!quizResult.IsSuccess)
        {
            return quizResult.ResultType.GetCommitResult(quizResult.ErrorCode, quizResult.ErrorMessage);
        }

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => request.CreateQuizRequest.Classes.Contains(a.Id) && a.IsActive)
                                                                   .Include(a => a.ClassEnrollees)
                                                                   .ToListAsync(cancellationToken);

        if (!teacherClasses.Any())
        {
            return ResultType.Empty.GetCommitResult("XTEC0007", _resourceJsonManager["XTEC0007"]);
        }

        EntityEntry<TeacherQuiz> teacherQuiz = _dbContext.Set<TeacherQuiz>().Add(new TeacherQuiz
        {
            ClipId = request.CreateQuizRequest.ClipId,
            Creator = _userId,
            Description = request.CreateQuizRequest.Description,
            StartDate = request.CreateQuizRequest.StartDate,
            EndDate = request.CreateQuizRequest.EndDate,
            Title = request.CreateQuizRequest.Title,
            QuizId = quizResult.Value,
            LessonName = request.CreateQuizRequest.LessonName,
            SubjectName = request.CreateQuizRequest.SubjectName,
            TeacherClasses = teacherClasses.ToList()
        });

        await _dbContext.SaveChangesAsync(cancellationToken);


        foreach (ClassEnrollee classEnrolled in teacherClasses.SelectMany(a => a.ClassEnrollees))
        {
            _dbContext.Set<TeacherQuizActivityTracker>().Add(new TeacherQuizActivityTracker
            {
                ClassId = classEnrolled.ClassId,
                ActivityStatus = ActivityStatus.New,
                StudentId = classEnrolled.StudentId,
                TeacherQuizId = teacherQuiz.Entity.Id
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}
