using TeacherDomain.Features.Quiz.CQRS.Command;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Command;
public class ReplyQuizCommandHandler : IRequestHandler<ReplyQuizCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly CurriculumClient _curriculumClient;
    public ReplyQuizCommandHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
    }
    public async Task<ICommitResult> Handle(ReplyQuizCommand request, CancellationToken cancellationToken)
    {
        ICommitResult? submitResult = await _curriculumClient.SubmitQuizeAsync(request.ReplyQuizRequest.StudentAnswers, cancellationToken);

        if (!submitResult.IsSuccess)
        {
            return submitResult;
        }

        TeacherQuizActivityTracker? teacherQuizActivityTracker = await _dbContext.Set<TeacherQuizActivityTracker>().SingleOrDefaultAsync(a => a.Id.Equals(request.ReplyQuizRequest.QuizActivityTrackerId), cancellationToken);

        if (teacherQuizActivityTracker == null)
        {
            return ResultType.NotFound.GetCommitResult("XXXXXX", "No Quiz Tracker by this Id");
        }

        teacherQuizActivityTracker.ActivityStatus = ActivityStatus.Finished;

        _dbContext.Set<TeacherQuizActivityTracker>().Update(teacherQuizActivityTracker);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}