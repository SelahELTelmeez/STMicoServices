using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Command;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Command;
public class UpdateStudentQuizCommandHandler : IRequestHandler<UpdateStudentQuizCommand, ICommitResult>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;

    public UpdateStudentQuizCommandHandler(StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<ICommitResult> Handle(UpdateStudentQuizCommand request, CancellationToken cancellationToken)
    {
        QuizTracker? quizTracker = await _dbContext.Set<QuizTracker>()
                                                   .FirstOrDefaultAsync(a => a.QuizId.Equals(request.UpdateStudentQuizRequest.QuizId) && a.StudentUserId.Equals(_userId), cancellationToken);
        if (quizTracker == null)
        {
            _dbContext.Set<QuizTracker>().Add(new QuizTracker
            {
                StudentUserId = _userId.GetValueOrDefault(),
                StudentUserScore = request.UpdateStudentQuizRequest.StudentUserScore,
                TimeSpentInSec = request.UpdateStudentQuizRequest.TimeSpentInSec,
                TotalQuizScore = request.UpdateStudentQuizRequest.TotalQuizScore,
                QuizId = request.UpdateStudentQuizRequest.QuizId,
                ClipId = request.UpdateStudentQuizRequest.ClipId,
                IsAnswered = request.UpdateStudentQuizRequest.IsAnswered,
            });
        }
        else
        {
            quizTracker.StudentUserScore = request.UpdateStudentQuizRequest.StudentUserScore;
            quizTracker.TimeSpentInSec = request.UpdateStudentQuizRequest.TimeSpentInSec;
            _dbContext.Set<QuizTracker>().Update(quizTracker);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}