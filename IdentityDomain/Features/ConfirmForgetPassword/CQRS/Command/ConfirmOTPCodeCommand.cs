using IdentityDomain.Features.ConfirmForgetPassword.DTO;
using ResultHandler;

namespace IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
public record ConfirmForgetPasswordCommand(ConfirmChangeEmailOrMobileOTPCodeDTO OTPCode) : IRequest<CommitResult<Guid>>;