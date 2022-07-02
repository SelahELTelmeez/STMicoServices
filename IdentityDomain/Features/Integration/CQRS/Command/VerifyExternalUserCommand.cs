using ResultHandler;

namespace IdentityDomain.Features.Integration.CQRS.Command;

public record VerifyExternalUserCommand(string ExternalUserId, string Provider) : IRequest<CommitResult<string>>;