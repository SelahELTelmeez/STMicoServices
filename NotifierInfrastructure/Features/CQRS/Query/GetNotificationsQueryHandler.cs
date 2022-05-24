using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Query;
using NotifierDomain.Features.DTO.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Query;
public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, CommitResults<NotificationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IdentityClient _IdentityClient;
    public GetNotificationsQueryHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<NotificationResponse>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Notification> notifications = await _dbContext.Set<Notification>()
                                                                    .Where(a => a.NotifiedId.Equals(_userId))
                                                                    .Include(a => a.NotificationTypeFK)
                                                                    .OrderByDescending(a => a.CreatedOn)
                                                                    .ToListAsync(cancellationToken);

        if (!notifications.Any())
        {
            return new CommitResults<NotificationResponse>
            {
                ResultType = ResultType.Empty,
                Value = Array.Empty<NotificationResponse>()
            };
        }

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _IdentityClient.GetLimitedProfilesAsync(notifications.Select(a => a.NotifierId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return new CommitResults<NotificationResponse>
            {
                ErrorCode = limitedProfiles.ErrorCode,
                ErrorMessage = limitedProfiles.ErrorMessage,
                ResultType = limitedProfiles.ResultType
            };
        }

        IEnumerable<NotificationResponse> Mapper()
        {
            foreach (Notification notification in notifications)
            {
                LimitedProfileResponse? notifierProfile = limitedProfiles.Value.SingleOrDefault(a => a.UserId.Equals(notification.NotifierId));

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
        return new CommitResults<NotificationResponse>()
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };

    }

}