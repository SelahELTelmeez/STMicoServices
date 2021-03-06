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
    public class GetIdentityLimitedProfilesQueryHandler : IRequestHandler<GetIdentityLimitedProfilesQuery, ICommitResults<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        public GetIdentityLimitedProfilesQueryHandler(STIdentityDbContext dbContext,
                                                                    IWebHostEnvironment configuration,
                                                                    IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResults<LimitedProfileResponse>> Handle(GetIdentityLimitedProfilesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<IdentityUser>? users = await _dbContext.Set<IdentityUser>()
                                                                .Include(a => a.GradeFK)
                                                                .Include(a => a.AvatarFK)
                                                                .Where(a => request.Ids.Contains(a.Id))
                                                                .ToListAsync(cancellationToken);
            if (!users.Any())
            {
                return ResultType.NotFound.GetValueCommitResults(Array.Empty<LimitedProfileResponse>(), "XIDN0017", _resourceJsonManager["XIDN0017"]);
            }

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

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }

}
