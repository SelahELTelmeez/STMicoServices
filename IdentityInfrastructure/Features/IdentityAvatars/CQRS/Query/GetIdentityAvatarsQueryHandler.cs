using IdentityDomain.Features.IdentityAvatars.CQRS.Query;
using IdentityDomain.Features.IdentityAvatars.DTO.Query;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Shared.Identities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityAvatars.CQRS.Query;

public class GetIdentityAvatarsQueryHandler : IRequestHandler<GetIdentityAvatarsQuery, CommitResults<IdentityAvatarResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public GetIdentityAvatarsQueryHandler(STIdentityDbContext dbContext,
                                  IWebHostEnvironment configuration,
                                  IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _mediator = mediator;
    }
    public async Task<CommitResults<IdentityAvatarResponse>> Handle(GetIdentityAvatarsQuery request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(request.UserId ?? _httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResults<IdentityAvatarResponse>
            {
                ErrorCode = "XIDN0001",
                ErrorMessage = _resourceJsonManager["XIDN0001"],
                ResultType = ResultType.NotFound,
            };
        }

        List<Avatar> avatars = await _dbContext.Set<Avatar>().ToListAsync(cancellationToken);

        return new CommitResults<IdentityAvatarResponse>
        {
            ResultType = ResultType.Ok,
            Value = avatars.Where(a => a.AvatarType == MapFromIdentityRoleToAvatarType(identityUser.IdentityRoleId)).Adapt<List<IdentityAvatarResponse>>()
        };
    }

    private string MapFromIdentityRoleToAvatarType(int? IdentityRoleId)
    {
        if (IdentityRoleId == 0)
        {
            return "Default";
        }
        if (IdentityRoleId == 1)
        {
            return "Student";
        }
        if (IdentityRoleId == 2)
        {
            return "Parent";
        }
        if (IdentityRoleId == 3)
        {
            return "Teacher";
        }
        return "Default";
    }
}
