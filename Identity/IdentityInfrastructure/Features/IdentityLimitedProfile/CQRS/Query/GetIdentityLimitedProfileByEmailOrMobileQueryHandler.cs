using Flaminco.CommitResult;
using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.HttpClients;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.IdentityLimitedProfile.CQRS.Query
{
    public class GetIdentityLimitedProfileByEmailOrMobileQueryHandler : IRequestHandler<GetIdentityLimitedProfileByEmailOrMobileQuery, ICommitResult<LimitedProfileResponse>>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly PaymentClient _paymentClient;

        public GetIdentityLimitedProfileByEmailOrMobileQueryHandler(STIdentityDbContext dbContext,
                                                                    IWebHostEnvironment configuration,
                                                                    IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResult<LimitedProfileResponse>> Handle(GetIdentityLimitedProfileByEmailOrMobileQuery request, CancellationToken cancellationToken)
        {

            IdentityUser? user = await _dbContext.Set<IdentityUser>()
                                                 .Include(a => a.AvatarFK)
                                                 .Include(a => a.GradeFK)
                                                 .Where(a => a.IdentityRoleId == 1)
                                                 .Where(a => string.IsNullOrWhiteSpace(request.MobileNumber) ? a.Email == request.Email : a.MobileNumber == request.MobileNumber)
                                                 .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                return ResultType.NotFound.GetValueCommitResult((LimitedProfileResponse)null, "XIDN0001", _resourceJsonManager["XIDN0001"]);
            }

            IdentityRelation? identityRelation = await _dbContext.Set<IdentityRelation>().FirstOrDefaultAsync(a => a.SecondaryId == user.Id, cancellationToken);

            if (identityRelation == null)
            {
                return ResultType.Ok.GetValueCommitResult(new LimitedProfileResponse
                {
                    FullName = user.FullName,
                    NotificationToken = user.NotificationToken,
                    UserId = user.Id,
                    AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}",
                    GradeId = user.GradeId.GetValueOrDefault(),
                    GradeName = user.GradeFK.Name,
                    IsPremium = user.IsPremium,
                });
            }
            else
            {
                return ResultType.Duplicated.GetValueCommitResult(new LimitedProfileResponse
                {
                    FullName = user.FullName,
                    NotificationToken = user.NotificationToken,
                    UserId = user.Id,
                    AvatarImage = $"https://selaheltelmeez.com/Media21-22/LMSApp/avatar/{user.AvatarFK.AvatarType}/{user.AvatarFK.ImageUrl}",
                    GradeId = user.GradeId.GetValueOrDefault(),
                    GradeName = user.GradeFK.Name,
                    IsPremium = user.IsPremium,
                });
            }
        }
    }
}
