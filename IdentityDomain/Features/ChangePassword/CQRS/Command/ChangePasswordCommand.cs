using Flaminco.CommitResult;
using IdentityDomain.Features.ChangePassword.DTO.Command;

namespace IdentityDomain.Features.ChangePassword.CQRS.Command;
public record ChangePasswordCommand(ChangePasswordRequest ChangePasswordRequest) : IRequest<ICommitResult>;