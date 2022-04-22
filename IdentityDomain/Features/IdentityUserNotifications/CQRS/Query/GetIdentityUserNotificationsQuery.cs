using IdentityDomain.Features.IdentityUserNotifications.DTO.Query;
using ResultHandler;

namespace IdentityDomain.Features.IdentityUserNotifications.CQRS.Query;

public record GetIdentityUserNotificationsQuery(IEnumerable<Guid> NotiferIds) : IRequest<CommitResults<IdentityUserNotificationResponse>>;


