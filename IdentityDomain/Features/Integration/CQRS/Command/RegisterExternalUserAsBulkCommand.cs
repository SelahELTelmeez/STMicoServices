using ResultHandler;

namespace IdentityDomain.Features.Integration.CQRS.Command;

public record RegisterExternalUserAsBulkCommand(Stream Stream, Guid ProviderSecretKey) : IRequest<CommitResult>;