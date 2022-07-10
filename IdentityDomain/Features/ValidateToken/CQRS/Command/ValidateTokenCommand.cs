using Flaminco.CommitResult;

namespace IdentityDomain.Features.ValidateToken.CQRS.Command;

public record ValidateTokenCommand() : IRequest<ICommitResult<bool>>;


