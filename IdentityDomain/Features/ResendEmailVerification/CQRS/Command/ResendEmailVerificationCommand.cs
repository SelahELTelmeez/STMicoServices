using ResultHandler;

namespace IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
public record ResendEmailVerificationCommand() : IRequest<CommitResult>;