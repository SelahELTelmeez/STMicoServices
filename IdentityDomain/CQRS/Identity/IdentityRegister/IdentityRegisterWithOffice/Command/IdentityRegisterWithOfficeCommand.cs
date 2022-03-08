using IdentityDomain.DTO.Identity.IdentityRegister.IdentityRegisterWithOffice.Command;
using IdentityDomain.DTO.Shared;
using MediatR;

namespace IdentityDomain.CQRS.Identity.IdentityRegister.IdentityRegisterWithOffice.Command;
public record IdentityRegisterWithOfficeCommand(IdentityRegisterWithOfficeDTO model) : IRequest<CommitResult<IdentityRegisterWithOfficeDTO>>;