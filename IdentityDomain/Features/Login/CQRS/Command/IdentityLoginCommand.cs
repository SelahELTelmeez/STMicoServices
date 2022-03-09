using IdentityDomain.Features.Login.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.Login.CQRS.Command;
public record IdentityLoginCommand(IdentityLoginRequestDTO IdentityLoginRequest) : IRequest<CommitResult<IdentityLoginResponseDTO>>;
