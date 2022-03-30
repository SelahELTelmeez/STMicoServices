using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.ConfirmChangeEmailOrMobile.CQRS.Command;

public record ConfirmChangeEmailOrMobileCommand(OTPVerificationRequest OTPVerificationRequest) : IRequest<CommitResult>;

