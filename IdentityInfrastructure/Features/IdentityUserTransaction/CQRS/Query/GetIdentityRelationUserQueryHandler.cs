using IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.IdentityUserTransaction.CQRS.Query;

public class GetIdentityRelationUserQueryHandler : IRequestHandler<GetIdentityRelationUserQuery, CommitResults<LimitedProfileResponse>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly Guid? _userId;

    public GetIdentityRelationUserQueryHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResults<LimitedProfileResponse>> Handle(GetIdentityRelationUserQuery Request, CancellationToken cancellationToken)
    {
        return new CommitResults<LimitedProfileResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<IdentityRelation>()
                           .Where(a => a.PrimaryId.Equals(_userId)
                                && a.RelationType == (RelationType)1)
                           .Include(a => a.SecondaryFK.GradeFK)
                           .Include(a => a.SecondaryFK.AvatarFK)
                           .Select(a => new LimitedProfileResponse
                           {
                               UserId = a.SecondaryId.Value,
                               FullName = a.SecondaryFK.FullName,
                               GradeName = a.SecondaryFK.GradeFK.Name,
                               GradeId = a.SecondaryFK.GradeId.Value,
                               AvatarImage = a.SecondaryFK.AvatarFK.ImageUrl
                           })
                           .ToListAsync(cancellationToken)
        };
    }
}