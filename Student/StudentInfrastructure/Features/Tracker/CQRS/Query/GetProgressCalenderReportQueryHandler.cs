using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO.Query;
using StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;

public class GetProgressCalenderReportQueryHandler : IRequestHandler<GetProgressCalenderReportQuery, ICommitResult<ProgressCalenderResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly string? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetProgressCalenderReportQueryHandler(IWebHostEnvironment configuration,
                                                 IHttpContextAccessor httpContextAccessor,
                                                 StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult<ProgressCalenderResponse>> Handle(GetProgressCalenderReportQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ActivityTracker> activityTrackers = await _dbContext.Set<ActivityTracker>()
                                                                        .Where(a => a.StudentId == (request.StudentId ?? _userId))
                                                                        .Where(a => a.CreatedOn <= DateTime.UtcNow.AddDays(28))
                                                                        .ToListAsync(cancellationToken);
        if (!activityTrackers.Any())
        {
            return ResultType.Empty.GetValueCommitResult<ProgressCalenderResponse>(default, "XSTU0001", _resourceJsonManager["XSTU0001"]);
        }
        return ResultType.Ok.GetValueCommitResult(new ProgressCalenderResponse
        {
            ActivityDates = activityTrackers.Select(a => a.CreatedOn.GetValueOrDefault()),
            MaxLearningDuration = activityTrackers.Max(a => a.LearningDurationInSec),
            MaxLearningDurationDate = activityTrackers.MaxBy(a => a.LearningDurationInSec).CreatedOn.GetValueOrDefault(),
            StartDate = activityTrackers.OrderByDescending(a => a.CreatedOn.GetValueOrDefault()).FirstOrDefault().CreatedOn.GetValueOrDefault()
        });
    }
}
