using ResultHandler;

namespace IdentityDomain.Features.ValidateToken.CQRS.Command;

public record ValidateTokenCommand() : IRequest<CommitResult<bool>>;


