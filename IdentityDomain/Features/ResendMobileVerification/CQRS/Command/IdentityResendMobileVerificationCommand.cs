using IdentityDomain.Features.MobileVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
public record IdentityResendMobileVerificationCommand() : IRequest<CommitResult<IdentityResendMobileVerificationResponseDTO>>;