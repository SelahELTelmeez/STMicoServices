using IdentityDomain.Features.ResetPassword.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResetPassword.CQRS.Command;
public record ResetPasswordCommand(ResetPasswordRequestDTO ResetPasswordRequest) : IRequest<CommitResult>;