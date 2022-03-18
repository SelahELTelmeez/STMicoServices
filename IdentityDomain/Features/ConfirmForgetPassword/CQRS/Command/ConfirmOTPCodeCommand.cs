using IdentityDomain.Features.ConfirmForgetPassword.DTO;
using ResultHandler;

namespace IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
public record ConfirmForgetPasswordCommand(ConfirmOTPCodeDTO OTPCode) : IRequest<CommitResult<Guid>>;