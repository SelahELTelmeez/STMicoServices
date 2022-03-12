using IdentityDomain.Features.Login.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.Login.CQRS.Command;
public record LoginCommand(LoginRequestDTO LoginRequest) : IRequest<CommitResult<LoginResponseDTO>>;
