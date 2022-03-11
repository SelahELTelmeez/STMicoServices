using IdentityDomain.Features.ResendMobileVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResendMobileVerification.CQRS.Command;
public record ResendMobileVerificationCommand(Guid IdentityUserId) : IRequest<CommitResult>;