using IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
public record ChangeEmailOrMobileCommand(ChangeEmailOrMobileRequest ChangeEmailOrMobileRequest) : IRequest<CommitResult>;