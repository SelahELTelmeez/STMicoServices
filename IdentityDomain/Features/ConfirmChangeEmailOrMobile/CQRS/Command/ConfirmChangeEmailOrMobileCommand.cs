using IdentityDomain.Features.ConfirmForgetPassword.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ConfirmChangeEmailOrMobile.CQRS.Command;

public record ConfirmChangeEmailOrMobileCommand(ConfirmChangeEmailOrMobileOTPCodeDTO OTPCode) : IRequest<CommitResult>;

