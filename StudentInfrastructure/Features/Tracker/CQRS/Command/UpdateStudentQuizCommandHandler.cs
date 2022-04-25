using StudentDomain.Features.Tracker.CQRS.Command;
using StudentEntities.Entities;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.Utilities;

namespace StudentInfrastructure.Features.Tracker.CQRS.Command;
public class UpdateStudentQuizCommandHandler : IRequestHandler<UpdateStudentQuizCommand, CommitResult>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;

    public UpdateStudentQuizCommandHandler(StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult> Handle(UpdateStudentQuizCommand request, CancellationToken cancellationToken)
    {
        StudentQuizTracker? quizTracker = await _dbContext.Set<StudentQuizTracker>()
                                              .SingleOrDefaultAsync(a => a.QuizId.Equals(request.UpdateStudentQuizRequest.QuizId) && a.StudentUserId.Equals(_userId), cancellationToken);
        if (quizTracker == null)
        {
            _dbContext.Set<StudentQuizTracker>().Add(new StudentQuizTracker
            {
                StudentUserId = _userId.GetValueOrDefault(),
                StudentUserScore = request.UpdateStudentQuizRequest.StudentUserScore,
                TimeSpentInSec = request.UpdateStudentQuizRequest.TimeSpentInSec,
                TotalQuizScore = request.UpdateStudentQuizRequest.TotalQuizScore,
                QuizId = request.UpdateStudentQuizRequest.QuizId,
            });
        }
        else
        {
            quizTracker.StudentUserScore = request.UpdateStudentQuizRequest.StudentUserScore;
            quizTracker.TimeSpentInSec = request.UpdateStudentQuizRequest.TimeSpentInSec;
            _dbContext.Set<StudentQuizTracker>().Update(quizTracker);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}