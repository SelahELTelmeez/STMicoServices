using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TransactionDomain.Features.Quiz.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.TeacherActivity;
using TransactionEntites.Entities.TeacherClasses;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Quiz.CQRS;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly CurriculumClient _curriculumClient;
    public CreateQuizCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
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
                                                           .Include(a => a.StudentEnrolls)
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

        foreach (StudentEnrollClass studentEnrolled in teacherClasses.SelectMany(a => a.StudentEnrolls))
        {
            _dbContext.Set<TeacherQuizActivityTracker>().Add(new TeacherQuizActivityTracker
            {
                ClassId = studentEnrolled.ClassId,
                ActivityStatus = ActivityStatus.New,
                StudentId = studentEnrolled.StudentId,
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
