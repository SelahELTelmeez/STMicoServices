using Flaminco.CommitResult;
using IdentityDomain.Features.ChangeEmailOrMobile.DTO.Command;

namespace IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
public record ChangeEmailOrMobileCommand(ChangeEmailOrMobileRequest ChangeEmailOrMobileRequest) : IRequest<ICommitResult>;