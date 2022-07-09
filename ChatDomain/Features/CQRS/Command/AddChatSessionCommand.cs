namespace ChatDomain.Features.CQRS.Command;

public record AddChatSessionCommand(string ConnectionId) : IRequest<ICommitResult>;
