using Flaminco.CommitResult;

namespace IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
public record ResendEmailVerificationCommand() : IRequest<ICommitResult>;