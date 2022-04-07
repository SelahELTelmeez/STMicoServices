using TransactionDomain.Features.Notification.DTO;

namespace TransactionDomain.Features.Notification.CQRS.Command;
public record CreateNotificationCommand(NotificationRequest NotificationRequest) : IRequest<CommitResult<NotificationResponse>>;