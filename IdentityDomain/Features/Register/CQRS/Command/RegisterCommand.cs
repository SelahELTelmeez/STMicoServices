using IdentityDomain.Features.Register.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.Register.CQRS.Command;
public record RegisterCommand(RegisterRequestDTO RegisterRequest) : IRequest<CommitResult<RegisterResponseDTO>>;