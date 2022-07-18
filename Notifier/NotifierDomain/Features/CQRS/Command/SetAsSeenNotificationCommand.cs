namespace NotifierDomain.Features.CQRS.Command;

public record SetAsSeenNotificationCommand : IRequest<ICommitResult>;

