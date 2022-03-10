using IdentityDomain.Features.ResendEmailVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
public record IdentityResendEmailVerificationCommand() : IRequest<CommitResult<IdentityResendEmailVerificationResponseDTO>>;