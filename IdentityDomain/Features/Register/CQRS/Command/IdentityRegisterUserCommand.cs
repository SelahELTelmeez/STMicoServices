using IdentityDomain.Features.Register.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.Register.CQRS.Command;
public record IdentityRegisterUserCommand(IdentityRegisterRequestDTO IdentityRegisterRequest) : IRequest<CommitResult<IdentityRegisterResponseDTO>>;