using Flaminco.CommitResult;

namespace IdentityDomain.Features.Integration.CQRS.Command;

public record VerifyExternalUserCommand(string ExternalUserId, Guid ProviderSecretKey) : IRequest<ICommitResult<string>>;