using ChatDomain.Features.DTO;

namespace ChatDomain.Features.CQRS.Query;

public record LoadMessagesP2PQuery(Guid PeerId) : IRequest<ICommitResults<ServerMessage>>;
