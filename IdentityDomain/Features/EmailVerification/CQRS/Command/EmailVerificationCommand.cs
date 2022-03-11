using IdentityDomain.Features.EmailVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.EmailVerification.CQRS.Command;
public record EmailVerificationCommand(EmailVerificationRequestDTO EmailVerificationRequest) : IRequest<CommitResult>;