using Microsoft.EntityFrameworkCore;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO.Query;
using StudentEntities.Entities;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetStudentQuizzesResultQueryHandler : IRequestHandler<GetStudentQuizzesResultQuery, CommitResults<StudentQuizResultResponse>>
{
    private readonly StudentDbContext _dbContext;
    public GetStudentQuizzesResultQueryHandler(StudentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResults<StudentQuizResultResponse>> Handle(GetStudentQuizzesResultQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<QuizTracker> studentQuizTrackers = await _dbContext.Set<QuizTracker>()
                                                                              .Where(a => request.StudentQuizResultRequest.QuizIds.Contains(a.QuizId) && a.StudentUserId.Equals(request.StudentQuizResultRequest.StudentId))
                                                                              .ToListAsync(cancellationToken);

        IEnumerable<StudentQuizResultResponse> Mapper()
        {
            foreach (QuizTracker tracker in studentQuizTrackers)
            {
                yield return new StudentQuizResultResponse
                {
                    QuizId = tracker.QuizId,
                    QuizScore = tracker.TotalQuizScore,
                    StudentScore = tracker.StudentUserScore
                };
            }
            yield break;
        }
        return new CommitResults<StudentQuizResultResponse>
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}
