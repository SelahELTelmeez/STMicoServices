using NotifierDomain.Features.DTO.Query;

namespace NotifierDomain.Features.CQRS.Query;
public record GetNotificationsQuery() : IRequest<CommitResults<NotificationResponse>>;