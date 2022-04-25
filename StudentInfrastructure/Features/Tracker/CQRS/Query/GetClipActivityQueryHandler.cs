using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO.Query;
using DomainEntities = StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetClipActivityQueryHandler : IRequestHandler<GetClipActivityQuery, CommitResults<ClipActivityResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _UserId;
    public GetClipActivityQueryHandler(IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _UserId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<ClipActivityResponse>> Handle(GetClipActivityQuery request, CancellationToken cancellationToken)
        => new CommitResults<ClipActivityResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntities.ActivityTracker>()
                                    .Where(a => request.ClipIds.Contains(a.ClipId) && a.StudentId.Equals(_UserId))
                                    .ProjectToType<ClipActivityResponse>()
                                    .ToListAsync(cancellationToken)
        };
}
