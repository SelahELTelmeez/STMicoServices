using IdentityDomain.Features.Register.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.Register.CQRS.Command;
public record RegisterCommand(RegisterRequest RegisterRequest) : IRequest<CommitResult<Guid>>;