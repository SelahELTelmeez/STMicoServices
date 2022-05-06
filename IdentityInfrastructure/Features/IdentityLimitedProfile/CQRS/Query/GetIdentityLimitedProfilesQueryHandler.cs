using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.Shared.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetIdentityLimitedProfilesQueryHandler : IRequestHandler<GetIdentityLimitedProfilesQuery, CommitResults<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        public GetIdentityLimitedProfilesQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommitResults<LimitedProfileResponse>> Handle(GetIdentityLimitedProfilesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityUser>? users = await _dbContext.Set<IdentityUser>()
                                                 .Include(a => a.GradeFK)
                                                 .Include(a => a.AvatarFK)
                                                 .Where(a => request.Ids.Contains(a.Id))
                                                 .ToListAsync(cancellationToken);

            IEnumerable<LimitedProfileResponse> Mapper()
            {
                foreach (IdentityUser user in users)
                {

                    yield return new LimitedProfileResponse
                    {
                        FullName = user.FullName,
                        GradeName = user.GradeFK.Name,
                        NotificationToken = user.NotificationToken,
                        GradeId = user.GradeId.GetValueOrDefault(),
                        UserId = user.Id,
                        AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}"
                    };
                }
                yield break;
            }

            return new CommitResults<LimitedProfileResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }

}
