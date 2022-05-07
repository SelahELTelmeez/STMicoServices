using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeacherDomain.Features.Quiz.CQRS.Command;
using TeacherEntites.Entities.Shared;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Command;
public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, CommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly CurriculumClient _curriculumClient;
    public CreateQuizCommandHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
    }
    public async Task<CommitResult> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {

        CommitResult<int>? quizResult = await _curriculumClient.CreateQuizeAsync(request.CreateQuizRequest.ClipId, cancellationToken);

        if (!quizResult.IsSuccess)
        {
            return quizResult.Adapt<CommitResult>();
        }

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                           .Where(a => request.CreateQuizRequest.Classes.Contains(a.Id))
                                                           .Include(a => a.ClassEnrollees)
                                                           .ToListAsync(cancellationToken);


        EntityEntry<TeacherQuiz> teacherQuiz = _dbContext.Set<TeacherQuiz>().Add(new TeacherQuiz
        {
            ClipId = request.CreateQuizRequest.ClipId,
            Creator = _userId.GetValueOrDefault(),
            Description = request.CreateQuizRequest.Description,
            StartDate = request.CreateQuizRequest.StartDate,
            EndDate = request.CreateQuizRequest.EndDate,
            Title = request.CreateQuizRequest.Title,
            QuizId = quizResult.Value,
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

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
