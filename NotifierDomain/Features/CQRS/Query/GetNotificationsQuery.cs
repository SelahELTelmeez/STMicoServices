using Flaminco.CommitResult;
using NotifierDomain.Features.DTO.Query;

namespace NotifierDomain.Features.CQRS.Query;
public record GetNotificationsQuery() : IRequest<ICommitResults<NotificationResponse>>;