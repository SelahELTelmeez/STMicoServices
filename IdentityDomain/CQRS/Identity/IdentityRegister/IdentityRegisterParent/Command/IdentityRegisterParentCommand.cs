using IdentityDomain.DTO.Identity.IdentityRegister.IdentityRegisterParent.Command;
using IdentityDomain.DTO.Shared;
using MediatR;

namespace IdentityDomain.CQRS.Identity.IdentityRegister.IdentityRegister.Command;
public record IdentityRegisterParentCommand(IdentityRegisterParentDTO model) : IRequest<CommitResult<IdentityRegisterParentDTO>>;