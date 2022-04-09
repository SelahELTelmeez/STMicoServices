using IdentityDomain.Features.IdentityUserInvitations.CQRS.Query;
using IdentityDomain.Features.IdentityUserInvitations.DTO.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityUserInvitations.CQRS.Query;
public class GetIdentityUserInvitationsQueryHandler : IRequestHandler<GetIdentityUserInvitationsQuery, CommitResults<IdentityUserInvitationResponse>>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserInvitationsQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResults<IdentityUserInvitationResponse>> Handle(GetIdentityUserInvitationsQuery request, CancellationToken cancellationToken)
    {

        List<IdentityUser> identityUsers = await _dbContext.Set<IdentityUser>().Where(a => request.InviterIds.Contains(a.Id))
                      .Include(a => a.AvatarFK)
                      .ToListAsync(cancellationToken);

        return new CommitResults<IdentityUserInvitationResponse>
        {
            ResultType = ResultType.Ok,
            Value = identityUsers.Select(a => new IdentityUserInvitationResponse
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