using NotifierDomain.Features.Notification.DTO.Query;

namespace NotifierDomain.Features.Notification.CQRS.Query;
public record GetNotificationsQuery() : IRequest<CommitResults<NotificationResponse>>;