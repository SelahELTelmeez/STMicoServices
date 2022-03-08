using IdentityDomain.DTO.Identity.IdentityLogin.Command;
using IdentityDomain.DTO.Shared;
using MediatR;

namespace IdentityDomain.CQRS.Identity.IdentityLogin.Command;
public record IdentityLoginCommand(IdentityLoginDTO model) : IRequest<CommitResult<IdentityLoginDTO>>;
