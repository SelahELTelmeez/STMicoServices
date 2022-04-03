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
        => await _dbContext.Set<IdentityUser>().Where(a => request.InviterIds.Contains(a.Id))
                           .Include(a=>a.AvatarFK)
                           .Select(a => new IdentityUserInvitationResponse
                            {
                                FullName = a.FullName,
                                Id = a.Id,
                                //Avatar = getAvatarUrl(a.AvatarFK)
                                Avatar = "" // There is issue in path of avatar
                            }).ToListAsync(cancellationToken);

    private string getAvatarUrl(Avatar? value)
        => value is null
            ? $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/default/default.png"
            : $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), value.AvatarType)}/{value.ImageUrl}";
}