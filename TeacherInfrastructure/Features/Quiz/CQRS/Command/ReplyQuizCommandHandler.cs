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
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ReplyQuizCommandHandler(TeacherDbContext dbContext,
                                   IHttpContextAccessor httpContextAccessor,
                                   IWebHostEnvironment configuration,
                                   CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<ICommitResult> Handle(ReplyQuizCommand request, CancellationToken cancellationToken)
    {
        ICommitResult? submitResult = await _curriculumClient.SubmitQuizeAsync(request.ReplyQuizRequest.StudentAnswers, cancellationToken);

        if (!submitResult.IsSuccess)
        {
            return submitResult;
        }

        TeacherQuizActivityTracker? teacherQuizActivityTracker = await _dbContext.Set<TeacherQuizActivityTracker>().FirstOrDefaultAsync(a => a.TeacherQuizId.Equals(request.ReplyQuizRequest.TeacherQuizId) && a.StudentId == _userId, cancellationToken);

        if (teacherQuizActivityTracker == null)
        {
            return ResultType.NotFound.GetCommitResult("XTEC0010", _resourceJsonManager["XTEC0010"]);
        }

        teacherQuizActivityTracker.ActivityStatus = ActivityStatus.Finished;

        _dbContext.Set<TeacherQuizActivityTracker>().Update(teacherQuizActivityTracker);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}