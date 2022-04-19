using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Notification.CQRS.Query;
using NotifierDomain.Features.Notification.DTO.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using NotifierInfrastructure.Utilities;

namespace NotifierInfrastructure.Features.Notifications.CQRS.Query;
public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, CommitResults<NotificationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly Guid? _userId;

    public GetAllNotificationsQueryHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<NotificationResponse>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<NotificationResponse> notificationResponses = await _dbContext.Set<Notification>()
                                     .Where(a => a.NotifierId.Equals(_userId))
                                     .ProjectToType<NotificationResponse>()
                                     .ToListAsync(cancellationToken);

        if (notificationResponses.Any())
        {
            return new CommitResults<NotificationResponse>()
            {
                ResultType = ResultType.Ok,
                Value = notificationResponses
            };
        }
        else
        {
            return new CommitResults<NotificationResponse>()
            {
                ResultType = ResultType.Empty,
            };
        }
    }

}