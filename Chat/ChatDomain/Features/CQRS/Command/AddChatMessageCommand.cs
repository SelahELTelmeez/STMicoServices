using ChatDomain.Features.DTO;

namespace ChatDomain.Features.CQRS.Command;

public record AddChatMessageCommand(ServerMessage ServerMessage) : IRequest<CommitResult>;
