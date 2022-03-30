using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
public record ConfirmForgetPasswordCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<CommitResult<Guid>>;