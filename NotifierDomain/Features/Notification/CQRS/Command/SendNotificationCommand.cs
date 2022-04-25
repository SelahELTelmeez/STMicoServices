using NotifierDomain.Features.Notification.DTO.Command;

namespace NotifierDomain.Features.Notification.CQRS.Command;
public record SendNotificationCommand(NotificationRequest NotificationRequest) : IRequest<CommitResult>;