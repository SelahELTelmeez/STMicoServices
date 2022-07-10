using Flaminco.CommitResult;
using IdentityDomain.Features.Integration.DTO;

namespace IdentityDomain.Features.Integration.CQRS.Command;

public record RegisterExternalUserCommand(ExternalUserRegisterRequest ExternalUserRegisterRequest, Guid ProviderSecretKey) : IRequest<ICommitResult<string>>;

