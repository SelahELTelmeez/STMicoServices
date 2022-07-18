using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierDomain.Features.CQRS.Query;
using NotifierDomain.Features.DTO.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Query;
public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, ICommitResults<NotificationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly string? _userId;
    private readonly IdentityClient _IdentityClient;
    private readonly IMediator _mediator;

    public GetNotificationsQueryHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient, IMediator mediator)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _IdentityClient = identityClient;
        _mediator = mediator;
    }
    public async Task<ICommitResults<NotificationResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Notification> notifications = await _dbContext.Set<Notification>()
                                                                  .Where(a => a.NotifiedId.Equals(_userId))
                                                                  .Include(a => a.NotificationTypeFK)
                                                                  .OrderByDescending(a => a.CreatedOn)
                                                                  .ToListAsync(cancellationToken);
        if (!notifications.Any())
        {
            return ResultType.Ok.GetValueCommitResults(Array.Empty<NotificationResponse>());
        }

        ICommitResults<LimitedProfileResponse>? limitedProfiles = await _IdentityClient.GetLimitedProfilesAsync(notifications.Select(a => a.NotifierId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return ResultType.Invalid.GetValueCommitResults<NotificationResponse>(default, limitedProfiles.ErrorCode, limitedProfiles.ErrorMessage);
        }

        IEnumerable<NotificationResponse> Mapper()
        {
            foreach (Notification notification in notifications)
            {
                LimitedProfileResponse? notifierProfile = limitedProfiles.Value.FirstOrDefault(a => a.UserId.Equals(notification.NotifierId));

                yield return new NotificationResponse
                {
                    CreatedOn = notification.CreatedOn.GetValueOrDefault(),
                    NotificationId = notification.Id,
                    IsSeen = notification.IsSeen,
                    Argument = notification.Argument,
                    Description = notification.Message,
                    AvatarUrl = notifierProfile.AvatarImage,
                    NotifiedId = notification.NotifiedId,
                    NotifierId = notification.NotifierId,
                    Type = notification.NotificationTypeId
                };
            }
            yield break;
        };

        await _mediator.Send(new SetAsSeenNotificationCommand(), cancellationToken);

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}