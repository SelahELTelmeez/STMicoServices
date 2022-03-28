using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.MobileVerification.CQRS.Command;
public record MobileVerificationCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<CommitResult>;