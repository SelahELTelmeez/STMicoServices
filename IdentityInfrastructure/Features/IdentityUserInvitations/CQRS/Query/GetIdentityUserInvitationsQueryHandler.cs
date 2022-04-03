using IdentityDomain.Features.IdentityUserInvitations.CQRS.Query;
using IdentityDomain.Features.IdentityUserInvitations.DTO.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Shared.Identities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.IdentityUserInvitations.CQRS.Query;
public class GetIdentityUserInvitationsQueryHandler : IRequestHandler<GetIdentityUserInvitationsQuery, List<IdentityUserInvitationResponse>>
{
    private readonly STIdentityDbContext _dbContext;

    public GetIdentityUserInvitationsQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<IdentityUserInvitationResponse>> Handle(GetIdentityUserInvitationsQuery request, CancellationToken cancellationToken)
    {

        List<IdentityUser> identityUsers = await _dbContext.Set<IdentityUser>().Where(a => request.InviterIds.Contains(a.Id))
                      .Include(a => a.AvatarFK)
                      .ToListAsync(cancellationToken);

        return identityUsers.Select(a => new IdentityUserInvitationResponse
        {
            FullName = a.FullName,
            Id = a.Id,
            Avatar = a.AvatarFK == null
                    ? $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/default/default.png"
                    : $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), a.AvatarFK.AvatarType)}/{a.AvatarFK.ImageUrl}"
        }).ToList();

    }
}