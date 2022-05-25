using IdentityDomain.Features.Login.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.UpdateNotificationToken.CQRS.Command;

public record UpdateNotificationTokenCommand(string NotificationToken) : IRequest<CommitResult<LoginResponse>>;


