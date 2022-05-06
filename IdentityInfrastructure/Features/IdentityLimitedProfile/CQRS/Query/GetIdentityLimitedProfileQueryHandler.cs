using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.Shared.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetIdentityLimitedProfileQueryHandler : IRequestHandler<GetIdentityLimitedProfileQuery, CommitResult<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        public GetIdentityLimitedProfileQueryHandler(STIdentityDbContext dbContext)

        {
            _dbContext = dbContext;
        }
        public async Task<CommitResult<LimitedProfileResponse>> Handle(GetIdentityLimitedProfileQuery request, CancellationToken cancellationToken)
        {
            IdentityUser? user = await _dbContext.Set<IdentityUser>()
                                                 .Include(a => a.GradeFK)
                                                 .Include(a => a.AvatarFK)
                                                 .SingleOrDefaultAsync(a => a.Id.Equals(request.Id));
            if (user == null)
            {
                return new CommitResult<LimitedProfileResponse>
                {
                    ResultType = ResultType.NotFound,
                };
            }
            return new CommitResult<LimitedProfileResponse>
            {
                ResultType = ResultType.Ok,
                Value = new LimitedProfileResponse
                {
                    FullName = user.FullName,
                    GradeName = user.GradeFK == null ? "" : user.GradeFK.Name,
                    NotificationToken = user.NotificationToken,
                    GradeId = user.GradeId.GetValueOrDefault(),
                    UserId = user.Id,
                    AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}"
                }
            };
        }
    }
}
