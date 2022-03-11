using IdentityDomain.Features.ResendEmailVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
public record ResendEmailVerificationCommand(Guid IdentityUserId) : IRequest<CommitResult>;