using Flaminco.CommitResult;
using IdentityDomain.Features.Shared.DTO;

namespace IdentityDomain.Features.MobileVerification.CQRS.Command;
public record MobileVerificationCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<ICommitResult>;