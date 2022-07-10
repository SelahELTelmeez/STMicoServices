using Flaminco.CommitResult;
using IdentityDomain.Features.Refresh.DTO.Command;

namespace IdentityDomain.Features.Refresh.CQRS.Command;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ICommitResult<RefreshTokenResponse>>;