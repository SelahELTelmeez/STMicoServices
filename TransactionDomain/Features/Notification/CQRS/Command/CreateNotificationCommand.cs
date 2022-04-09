using TransactionDomain.Features.Notification.DTO.Command;

namespace TransactionDomain.Features.Notification.CQRS.Command;
public record CreateNotificationCommand(NotificationRequest NotificationRequest) : IRequest<CommitResult<NotificationResponse>>;