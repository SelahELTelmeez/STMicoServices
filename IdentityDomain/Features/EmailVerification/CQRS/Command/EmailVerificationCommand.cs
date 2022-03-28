using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.EmailVerification.CQRS.Command;
public record EmailVerificationCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<CommitResult>;