using ResultHandler;

namespace IdentityDomain.Features.Integration.CQRS.Command;

public record VerifyExternalUserCommand(string ExternalUserId, Guid ProviderSecretKey) : IRequest<CommitResult<string>>;