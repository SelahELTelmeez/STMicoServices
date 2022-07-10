using Flaminco.CommitResult;
using IdentityDomain.Features.ResetPassword.DTO;
using ResultHandler;

namespace IdentityDomain.Features.ResetPassword.CQRS.Command;
public record ResetPasswordCommand(ResetPasswordRequest ResetPasswordRequest) : IRequest<ICommitResult>;