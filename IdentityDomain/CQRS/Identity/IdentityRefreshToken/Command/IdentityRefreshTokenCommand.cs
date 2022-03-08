using IdentityDomain.DTO.Identity.IdentityRefreshToken.Command;
using IdentityDomain.DTO.Shared;
using MediatR;

namespace IdentityDomain.CQRS.Identity.IdentityRefreshToken.Command;
public record IdentityRefreshTokenCommand(IdentityRefreshTokenDTO model) : IRequest<CommitResult<IdentityRefreshTokenDTO>>;