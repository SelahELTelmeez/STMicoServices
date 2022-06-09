using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetIdentityLimitedProfileByEmailOrMobileQueryHandler : IRequestHandler<GetIdentityLimitedProfileByEmailOrMobileQuery, CommitResult<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        public GetIdentityLimitedProfileByEmailOrMobileQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommitResult<LimitedProfileResponse>> Handle(GetIdentityLimitedProfileByEmailOrMobileQuery request, CancellationToken cancellationToken)
        {

            IdentityUser? user = await _dbContext.Set<IdentityUser>()
                                                 .Include(a => a.AvatarFK)
                                                 .Include(a => a.GradeFK)
                                                 .Where(a => a.IdentityRoleId == 1)
                                                 .Where(a => string.IsNullOrWhiteSpace(request.MobileNumber) ? a.Email == request.Email : a.MobileNumber == request.MobileNumber)
                                                 .SingleOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                return new CommitResult<LimitedProfileResponse>
                {
                    ResultType = ResultType.NotFound,
                };
            }

            IdentityRelation? identityRelation = await _dbContext.Set<IdentityRelation>().SingleOrDefaultAsync(a => a.SecondaryId == user.Id, cancellationToken);

            if (identityRelation == null)
            {
                return new CommitResult<LimitedProfileResponse>
                {
                    ResultType = ResultType.Ok,
                    Value = new LimitedProfileResponse
                    {
                        FullName = user.FullName,
                        NotificationToken = user.NotificationToken,
                        UserId = user.Id,
                        AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}",
                        GradeId = user.GradeId.GetValueOrDefault(),
                        GradeName = user.GradeFK.Name,
                        IsPremium = user.IsPremium
                    }
                };
            }
            else
            {
                return new CommitResult<LimitedProfileResponse>
                {
                    ResultType = ResultType.Duplicated,
                    Value = new LimitedProfileResponse
                    {
                        FullName = user.FullName,
                        NotificationToken = user.NotificationToken,
                        UserId = user.Id,
                        AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}",
                        GradeId = user.GradeId.GetValueOrDefault(),
                        GradeName = user.GradeFK.Name,
                        IsPremium = user.IsPremium
                    }
                };
            }


        }
    }
}
