using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Notification.CQRS.Query;
using TransactionDomain.Features.Notification.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Notification;

namespace TransactionInfrastructure.Features.Notification.CQRS.Query;
public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, CommitResults<NotificationResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;

    public GetAllNotificationsQueryHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<NotificationResponse>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
        => new CommitResults<NotificationResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntities.Notification>()
                                   .Where(a => a.NotifierId.Equals(_userId))
                                   .ProjectToType<NotificationResponse>()
                                   .ToListAsync(cancellationToken)
        };
}