using ResultHandler;

namespace IdentityDomain.Features.Shared.RemoveOldOTP.CQRS.Command;
public record RemoveOldOTPCommand(Guid? IdentityUserId) : IRequest<CommitResult>;