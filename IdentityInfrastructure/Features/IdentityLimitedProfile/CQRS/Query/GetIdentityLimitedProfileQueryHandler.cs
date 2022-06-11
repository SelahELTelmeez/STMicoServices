using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetIdentityLimitedProfileQueryHandler : IRequestHandler<GetIdentityLimitedProfileQuery, CommitResult<LimitedProfileResponse>>
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
                    ErrorCode = "X0001",
                    ErrorMessage = _resourceJsonManager["X0001"],
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
