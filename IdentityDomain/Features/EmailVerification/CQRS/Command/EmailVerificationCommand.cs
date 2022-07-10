using Flaminco.CommitResult;
using IdentityDomain.Features.Shared.DTO;

namespace IdentityDomain.Features.EmailVerification.CQRS.Command;
public record EmailVerificationCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<ICommitResult>;