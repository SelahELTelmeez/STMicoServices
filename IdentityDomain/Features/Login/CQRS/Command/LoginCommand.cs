using IdentityDomain.Features.Login.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.Login.CQRS.Command;
public record LoginCommand(LoginRequest LoginRequest) : IRequest<CommitResult<LoginResponse>>;
