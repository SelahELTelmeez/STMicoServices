using Flaminco.CommitResult;
using IdentityDomain.Features.Register.DTO.Command;

namespace IdentityDomain.Features.Register.CQRS.Command;
public record RegisterCommand(RegisterRequest RegisterRequest) : IRequest<ICommitResult<Guid>>;