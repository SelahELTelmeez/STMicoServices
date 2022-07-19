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
    public class GetIdentityLimitedProfileQueryHandler : IRequestHandler<GetIdentityLimitedProfileQuery, ICommitResult<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        public GetIdentityLimitedProfileQueryHandler(STIdentityDbContext dbContext,
                                                                    IWebHostEnvironment configuration,
                                                                    IHttpContextAccessor httpContextAccessor)

        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResult<LimitedProfileResponse>> Handle(GetIdentityLimitedProfileQuery request, CancellationToken cancellationToken)
        {
            IdentityUser? user = await _dbContext.Set<IdentityUser>()
                                                 .Include(a => a.GradeFK)
                                                 .Include(a => a.AvatarFK)
                                                 .FirstOrDefaultAsync(a => a.Id.Equals(request.Id));
            if (user == null)
            {
                return ResultType.NotFound.GetValueCommitResult<LimitedProfileResponse>(default, "XIDN0001", _resourceJsonManager["XIDN0001"]);
            }

            return ResultType.Ok.GetValueCommitResult(new LimitedProfileResponse
            {
                FullName = user.FullName,
                GradeName = user.GradeFK == null ? "" : user.GradeFK.Name,
                NotificationToken = user.NotificationToken,
                GradeId = user.GradeId.GetValueOrDefault(),
                UserId = user.Id,
                AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}",
                IsPremium = user.IsPremium,
            });
        }
    }
}
