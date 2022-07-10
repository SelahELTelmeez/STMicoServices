using Flaminco.CommitResult;
using IdentityDomain.Features.Login.DTO.Command;

namespace IdentityDomain.Features.Login.CQRS.Command;
public record LoginCommand(LoginRequest LoginRequest) : IRequest<ICommitResult<LoginResponse>>;
