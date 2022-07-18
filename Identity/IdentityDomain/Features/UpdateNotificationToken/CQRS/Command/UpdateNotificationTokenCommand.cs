using Flaminco.CommitResult;
using IdentityDomain.Features.Login.DTO.Command;

namespace IdentityDomain.Features.UpdateNotificationToken.CQRS.Command;

public record UpdateNotificationTokenCommand(string NotificationToken) : IRequest<ICommitResult<LoginResponse>>;


