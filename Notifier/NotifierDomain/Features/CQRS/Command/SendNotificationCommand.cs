using SharedModule.DTO;

namespace NotifierDomain.Features.CQRS.Command;
public record SendNotificationCommand(NotificationRequest NotificationRequest) : IRequest<ICommitResult>;