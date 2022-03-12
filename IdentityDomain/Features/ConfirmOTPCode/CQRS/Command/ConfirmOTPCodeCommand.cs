using ResultHandler;

namespace IdentityDomain.Features.ConfirmOTPCode.CQRS.Command;
public record ConfirmOTPCodeCommand(string OTPCode) : IRequest<CommitResult<Guid>>;