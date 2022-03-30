using IdentityDomain.Features.IdentityAvatars.CQRS.Query;
using IdentityDomain.Features.IdentityAvatars.DTO.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Shared.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.GetAvatars.CQRS.Query;

public class GetIdentityAvatarsQueryHandler : IRequestHandler<GetIdentityAvatarsQuery, CommitResult<List<IdentityAvatarResponse>>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetIdentityAvatarsQueryHandler(STIdentityDbContext dbContext,
                                  IWebHostEnvironment configuration,
                                  IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult<List<IdentityAvatarResponse>>> Handle(GetIdentityAvatarsQuery request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id == _httpContextAccessor.GetIdentityUserId(), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<List<IdentityAvatarResponse>>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], 
                ResultType = ResultType.NotFound,
            };
        }

        List<Avatar> avatars = await _dbContext.Set<Avatar>().ToListAsync(cancellationToken);
        return new CommitResult<List<IdentityAvatarResponse>>
        {
            ResultType = ResultType.Ok,
            Value = avatars.Where(a => a.AvatarType == MapFromIdentityRoleToAvatarType(identityUser.IdentityRoleId)).Adapt<List<IdentityAvatarResponse>>()
        };
    }

    private AvatarType MapFromIdentityRoleToAvatarType(int? IdentityRoleId)
    {
        if (IdentityRoleId == 0)
        {
            return AvatarType.Default;
        }
        if (IdentityRoleId == 1)
        {
            return AvatarType.Student;
        }
        if (IdentityRoleId == 2)
        {
            return AvatarType.Parent;
        }
        if (IdentityRoleId == 3)
        {
            return AvatarType.Teacher;
        }
        return AvatarType.Default;
    }
}
