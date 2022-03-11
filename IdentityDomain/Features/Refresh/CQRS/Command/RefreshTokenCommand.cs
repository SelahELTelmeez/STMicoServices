using IdentityDomain.Features.Refresh.DTO.Command;
using ResultHandler;
namespace IdentityDomain.Features.Refresh.CQRS.Command;
public record RefreshTokenCommand(string RefreshToken) : IRequest<CommitResult<RefreshTokenResponseDTO>>;