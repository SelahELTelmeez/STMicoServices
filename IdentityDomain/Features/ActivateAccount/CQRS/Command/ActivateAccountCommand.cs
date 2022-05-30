using ResultHandler;

namespace IdentityDomain.Features.ActivateAccount.CQRS.Command;

public record ActivateAccountCommand() : IRequest<CommitResult>;


