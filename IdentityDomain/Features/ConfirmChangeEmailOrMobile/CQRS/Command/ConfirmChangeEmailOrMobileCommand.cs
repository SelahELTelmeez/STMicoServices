using Flaminco.CommitResult;
using IdentityDomain.Features.Shared.DTO;

namespace IdentityDomain.Features.ConfirmChangeEmailOrMobile.CQRS.Command;

public record ConfirmChangeEmailOrMobileCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<ICommitResult>;

