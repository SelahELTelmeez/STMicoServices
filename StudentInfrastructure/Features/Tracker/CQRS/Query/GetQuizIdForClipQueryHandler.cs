using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetQuizIdForClipQueryHandler : IRequestHandler<GetQuizIdForClipQuery, ICommitResult<int?>>
    {
        private readonly StudentDbContext _dbContext;
        private readonly Guid? _userId;
        public GetQuizIdForClipQueryHandler(StudentDbContext dbContext,
                                            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<ICommitResult<int?>> Handle(GetQuizIdForClipQuery request, CancellationToken cancellationToken)
        {
            QuizTracker? quizTracker = await _dbContext.Set<QuizTracker>()
                .FirstOrDefaultAsync(a => a.ClipId == request.ClipId && a.StudentUserId == _userId, cancellationToken);

            if (quizTracker == null)
            {
                return ResultType.Ok.GetValueCommitResult<int?>(default);
            }
            else
            {
                return ResultType.Ok.GetValueCommitResult<int?>(quizTracker.QuizId);
            }
        }
    }
}
