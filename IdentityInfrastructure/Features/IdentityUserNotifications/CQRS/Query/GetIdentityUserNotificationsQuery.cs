using IdentityDomain.Features.IdentityUserNotifications.CQRS.Query;
using IdentityDomain.Features.IdentityUserNotifications.DTO.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityUserNotifications.CQRS.Query;

public class GetIdentityUserNotificationsQueryHandler : IRequestHandler<GetIdentityUserNotificationsQuery, CommitResults<IdentityUserNotificationResponse>>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserNotificationsQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResults<IdentityUserNotificationResponse>> Handle(GetIdentityUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<IdentityUser> identityUsers = await _dbContext.Set<IdentityUser>().Where(a => request.NotiferIds.Contains(a.Id))
                              .Include(a => a.AvatarFK)
                              .ToListAsync(cancellationToken);

        return new CommitResults<IdentityUserNotificationResponse>
        {
            ResultType = ResultType.Ok,
            Value = identityUsers.Select(a => new IdentityUserNotificationResponse
            {
                FullName = a.FullName,
                Id = a.Id,
                Avatar = a.AvatarFK == null
                    ? $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/default/default.png"
                    : $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), a.AvatarFK.AvatarType)}/{a.AvatarFK.ImageUrl}"
            })
        };
    }
}



