using ResultHandler;

namespace IdentityDomain.Features.Shared.RemoveOldOTP.CQRS.Command;
public record RemoveOldOTPCommand(string? IdentityUserId) : IRequest<CommitResult>;