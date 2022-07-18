using Flaminco.CommitResult;
using IdentityDomain.Features.Shared.DTO;

namespace IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
public record ConfirmForgetPasswordCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<ICommitResult<string>>;