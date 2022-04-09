using TransactionDomain.Features.Notification.DTO.Query;

namespace TransactionDomain.Features.Notification.CQRS.Query;
public record GetAllNotificationsQuery() : IRequest<CommitResults<NotificationResponse>>;