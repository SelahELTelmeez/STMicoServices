using ResultHandler;

namespace IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
public record ResendMobileVerificationCommand() : IRequest<CommitResult>;