using ResultHandler;

namespace IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
public record ConfirmForgetPasswordCommand(string OTPCode) : IRequest<CommitResult<Guid>>;