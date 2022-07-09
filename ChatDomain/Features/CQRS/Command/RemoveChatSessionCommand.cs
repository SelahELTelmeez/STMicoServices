namespace ChatDomain.Features.CQRS.Command;

public record RemoveChatSessionCommand(string ConnectionId) : IRequest<ICommitResult>;