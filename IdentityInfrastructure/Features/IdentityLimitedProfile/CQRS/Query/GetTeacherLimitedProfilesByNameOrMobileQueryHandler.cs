using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.Shared.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetTeacherLimitedProfilesByNameOrMobileQueryHandler : IRequestHandler<GetTeacherLimitedProfilesByNameOrMobileQuery, CommitResults<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        public GetTeacherLimitedProfilesByNameOrMobileQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommitResults<LimitedProfileResponse>> Handle(GetTeacherLimitedProfilesByNameOrMobileQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityUser>? users = await _dbContext.Set<IdentityUser>()
                                                             .Include(a => a.AvatarFK)
                                                             .Where(a => a.IdentityRoleId.Equals(3) && (a.FullName == request.NameOrMobileNumber || a.MobileNumber == request.NameOrMobileNumber))
                                                             .ToListAsync(cancellationToken);
            if (users == null)
            {
                return new CommitResults<LimitedProfileResponse>
                {
                    ResultType = ResultType.NotFound,
                };
            }

            IEnumerable<LimitedProfileResponse> Mapper()
            {
                foreach (IdentityUser user in users)
                {
                    yield return new LimitedProfileResponse
                    {
                        FullName = user.FullName,
                        NotificationToken = user.NotificationToken,
                        UserId = user.Id,
                        AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{Enum.GetName(typeof(AvatarType), user.AvatarFK.AvatarType)}/{user.AvatarFK.ImageUrl}"

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
