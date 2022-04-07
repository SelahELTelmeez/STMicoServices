using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Tracker.CQRS.Query;
using TransactionDomain.Features.Tracker.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Trackers;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Query;
public class GetClipActivityQueryHandler : IRequestHandler<GetClipActivityQuery, CommitResults<ClipActivityResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _UserId;
    public GetClipActivityQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _UserId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<ClipActivityResponse>> Handle(GetClipActivityQuery request, CancellationToken cancellationToken)
        => new CommitResults<ClipActivityResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntities.StudentActivityTracker>()
                                    .Where(a => request.ClipIds.Contains(a.ClipId) && a.StudentId.Equals(_UserId))
                                    .ProjectToType<ClipActivityResponse>()
                                    .ToListAsync(cancellationToken)
        };
}
