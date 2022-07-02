using IdentityDomain.Features.Integration.DTO;
using ResultHandler;

namespace IdentityDomain.Features.Integration.CQRS.Command;

public record RegisterExternalUserCommand(ExternalUserRegisterRequest ExternalUserRegisterRequest) : IRequest<CommitResult<string>>;

