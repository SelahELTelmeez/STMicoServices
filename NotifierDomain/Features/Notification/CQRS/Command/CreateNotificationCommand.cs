using NotifierDomain.Features.Notification.DTO.Command;

namespace NotifierDomain.Features.Notification.CQRS.Command;
public record CreateNotificationCommand(NotificationRequest NotificationRequest) : IRequest<CommitResult>;