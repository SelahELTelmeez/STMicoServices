using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Invitations.CQRS.DTO.Query;
using NotifierDomain.Features.Notification.CQRS.Query;
using NotifierDomain.Features.Notification.DTO.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using NotifierInfrastructure.HttpClients;
using NotifierInfrastructure.Utilities;

namespace NotifierInfrastructure.Features.Notifications.CQRS.Query;
public class GetAllNotificationsQueryHandler : IRequestHandler<GetAllNotificationsQuery, CommitResults<NotificationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IdentityClient _IdentityClient;
    public GetAllNotificationsQueryHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<NotificationResponse>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Notification> notifications = await _dbContext.Set<Notification>()
                                                                    .Where(a => a.NotifiedId.Equals(_userId))
                                                                    .Include(a => a.NotificationTypeFK)
                                                                    .OrderByDescending(a => a.CreatedOn)
                                                                    .ToListAsync(cancellationToken);

        CommitResults<IdentityUserNotificationResponse>? identityUserInvitationResponses = await _IdentityClient.GetIdentityUserNotificationsAsync(notifications.Select(a => a.NotifierId), cancellationToken);


        IEnumerable<NotificationResponse> Mapper()
        {
            foreach (Notification notification in notifications)
            {
                IdentityUserNotificationResponse notifierProfile = identityUserInvitationResponses.Value.SingleOrDefault(a => a.Id.Equals(notification.NotifierId));

                yield return new NotificationResponse
                {
                    CreatedOn = notification.CreatedOn.GetValueOrDefault(),
                    NotificationId = notification.Id,
                    IsSeen = notification.IsSeen,
                    Argument = notification.Argument,
                    Description = $"{notification.NotificationTypeFK.Name} {notifierProfile.FullName} {notification.NotificationTypeFK.Description}",
                    AvatarUrl = notifierProfile.Avatar
                };
            }
            yield break;
        };
        return new CommitResults<NotificationResponse>()
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };

    }

}