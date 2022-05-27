using Flaminco.CommitResult;
using NotifierDomain.Features.DTO.Command;

namespace NotifierDomain.Features.CQRS.Command;
public record SendNotificationCommand(NotificationRequest NotificationRequest) : IRequest<ICommitResult>;