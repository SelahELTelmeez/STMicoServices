using IdentityDomain.DTO.Identity.IdentityRegister.IdentityRegisterUser.Command;
using IdentityDomain.DTO.Shared;
using MediatR;

namespace IdentityDomain.CQRS.Identity.IdentityRegister.IdentityRegisterUser.Command;
public record IdentityRegisterUserCommand(IdentityRegisterUserDTO model) : IRequest<CommitResult<IdentityRegisterUserDTO>>;