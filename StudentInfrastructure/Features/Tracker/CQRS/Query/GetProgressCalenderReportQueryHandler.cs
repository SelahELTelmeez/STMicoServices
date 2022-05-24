using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetProgressCalenderReportQueryHandler : IRequestHandler<GetProgressCalenderReportQuery, CommitResult<ProgressCalenderResponse>>
    {
        private readonly StudentDbContext _dbContext;
        private readonly Guid? _UserId;
        public GetProgressCalenderReportQueryHandler(IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
        {
            _dbContext = dbContext;
            _UserId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<CommitResult<ProgressCalenderResponse>> Handle(GetProgressCalenderReportQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ActivityTracker> activityTrackers = await _dbContext.Set<ActivityTracker>()
                                                                            .Where(a => a.StudentId == (request.StudentId ?? _UserId.GetValueOrDefault()))
                                                                            .Where(a => a.CreatedOn <= DateTime.UtcNow.AddDays(28))
                                                                            .ToListAsync(cancellationToken);

            return new CommitResult<ProgressCalenderResponse>
            {
                ResultType = ResultType.Ok,
                Value = new ProgressCalenderResponse
                {
                    ActivityDates = activityTrackers.Select(a => a.CreatedOn.GetValueOrDefault()),
                    MaxLearningDuration = activityTrackers.Max(a => a.LearningDurationInSec),
                    MaxLearningDurationDate = activityTrackers.MaxBy(a => a.LearningDurationInSec).CreatedOn.GetValueOrDefault(),
                    StartDate = activityTrackers.OrderByDescending(a => a.CreatedOn.GetValueOrDefault()).FirstOrDefault().CreatedOn.GetValueOrDefault()
                }
            };
        }
    }
}
