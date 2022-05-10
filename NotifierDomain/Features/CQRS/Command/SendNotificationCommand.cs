using NotifierDomain.Features.DTO.Command;

namespace NotifierDomain.Features.CQRS.Command;
public record SendNotificationCommand(NotificationRequest NotificationRequest) : IRequest<CommitResult>;