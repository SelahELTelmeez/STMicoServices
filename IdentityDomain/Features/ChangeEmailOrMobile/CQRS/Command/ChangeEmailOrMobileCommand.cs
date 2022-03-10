using IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
public record ChangeEmailOrMobileCommand(ChangeEmailOrMobileRequestDTO ChangeEmailOrMobileRequest) : IRequest<CommitResult>;