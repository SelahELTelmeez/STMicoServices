﻿using IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;
using IdentityDomain.Features.Shared.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

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
    => new CommitResults<LimitedProfileResponse>
    {
        ResultType = ResultType.Ok,
        Value = await _dbContext.Set<IdentityRelation>()
                           .Where(a => a.PrimaryId.Equals(_userId)
                                && a.RelationType.Equals(1))
                           .Include(a => a.SecondaryFK.GradeFK)
                           .Include(a => a.SecondaryFK.AvatarFK)
                           .Select(a => new LimitedProfileResponse
                           {
                               UserId = a.SecondaryId.Value,
                               FullName = a.SecondaryFK.FullName,
                               GradeName = a.SecondaryFK.GradeFK.Name,
                               GradeId = a.SecondaryFK.GradeId.Value,
                               AvatarImage = a.SecondaryFK.AvatarFK.ImageUrl,
                               IsPremium = a.SecondaryFK.IsPremium,
                               //NotificationToken = a.NotificationToken,
                           })
                           .ToListAsync(cancellationToken)
    };
}