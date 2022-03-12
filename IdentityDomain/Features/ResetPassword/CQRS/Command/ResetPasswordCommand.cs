using IdentityDomain.Features.ResetPassword.DTO;
using ResultHandler;

namespace IdentityDomain.Features.ResetPassword.CQRS.Command;
public record ResetPasswordCommand(ResetPasswordRequestDTO ResetPasswordRequest) : IRequest<CommitResult>;