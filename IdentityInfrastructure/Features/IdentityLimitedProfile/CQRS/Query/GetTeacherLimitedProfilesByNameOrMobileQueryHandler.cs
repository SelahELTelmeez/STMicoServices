using Flaminco.CommitResult;
using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetTeacherLimitedProfilesByNameOrMobileQueryHandler : IRequestHandler<GetTeacherLimitedProfilesByNameOrMobileQuery, ICommitResults<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        public GetTeacherLimitedProfilesByNameOrMobileQueryHandler(STIdentityDbContext dbContext,
                                                                    IWebHostEnvironment configuration,
                                                                    IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResults<LimitedProfileResponse>> Handle(GetTeacherLimitedProfilesByNameOrMobileQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityUser>? users = await _dbContext.Set<IdentityUser>()
                                                               .Include(a => a.AvatarFK)
                                                               .Where(a => a.IdentityRoleId.Equals(3) && (a.FullName == request.NameOrMobileNumber || a.MobileNumber == request.NameOrMobileNumber))
                                                               .ToListAsync(cancellationToken);
            if (users == null)
            {
                return ResultType.NotFound.GetValueCommitResults(Array.Empty<LimitedProfileResponse>(), "XIDN0001", _resourceJsonManager["XIDN0001"]);
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
                        AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}",
                    };
                }
                yield break;
            }
            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}
