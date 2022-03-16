using IdentityDomain.Features.IdentityGrade.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityGrade.CQRS.Query;

public class GetIdentityGradeQueryHandler : IRequestHandler<GetIdentityGradeQuery, CommitResult<int>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetIdentityGradeQueryHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _resourceJsonManager = resourceJsonManager;
    }
    public async Task<CommitResult<int>> Handle(GetIdentityGradeQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<int>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        if (identityUser.IdentityRoleId != 1)
        {
            return new CommitResult<int>
            {
                ErrorCode = "X0010",
                ErrorMessage = _resourceJsonManager["X0010"], // only student can have grade.
                ResultType = ResultType.Invalid,
            };
        }
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = identityUser.GradeId.GetValueOrDefault(),
        };
    }
}

