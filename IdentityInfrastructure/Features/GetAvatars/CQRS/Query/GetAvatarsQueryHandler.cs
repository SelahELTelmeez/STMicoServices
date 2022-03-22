using IdentityDomain.Features.GetAvatars.CQRS.Query;
using IdentityDomain.Features.GetAvatars.DTO.Query;
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

public class GetAvatarsQueryHandler : IRequestHandler<GetAvatarsQuery, CommitResult<List<AvatarResponseDTO>>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetAvatarsQueryHandler(STIdentityDbContext dbContext,
                                  IWebHostEnvironment configuration,
                                  IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult<List<AvatarResponseDTO>>> Handle(GetAvatarsQuery request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id == _httpContextAccessor.GetIdentityUserId(), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<List<AvatarResponseDTO>>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], 
                ResultType = ResultType.NotFound,
            };
        }

        List<Avatar> avatars = await _dbContext.Set<Avatar>().ToListAsync(cancellationToken);
        return new CommitResult<List<AvatarResponseDTO>>
        {
            ResultType = ResultType.Ok,
            Value = avatars.Where(a => a.AvatarType == MapFromIdentityRoleToAvatarType(identityUser.IdentityRoleId)).Adapt<List<AvatarResponseDTO>>()
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
