using Flaminco.CommitResult;

namespace IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
public record ResendMobileVerificationCommand() : IRequest<ICommitResult>;