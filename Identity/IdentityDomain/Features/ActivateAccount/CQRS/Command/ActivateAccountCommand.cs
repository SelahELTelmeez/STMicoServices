using Flaminco.CommitResult;

namespace IdentityDomain.Features.ActivateAccount.CQRS.Command;

public record ActivateAccountCommand() : IRequest<ICommitResult>;


