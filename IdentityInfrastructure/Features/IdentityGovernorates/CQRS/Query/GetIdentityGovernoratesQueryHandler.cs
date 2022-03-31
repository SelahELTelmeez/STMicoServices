﻿using IdentityDomain.Features.IdentityGovernorates.CQRS.Query;
using IdentityDomain.Features.IdentityGovernorates.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Locations;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityGovernorates.CQRS.Query;

public class GetIdentityGovernoratesQueryHandler : IRequestHandler<GetIdentityGovernoratesQuery, CommitResult<List<IdentityGovernorateResponse>>>
{
    private readonly STIdentityDbContext _dbContext;
    public GetIdentityGovernoratesQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult<List<IdentityGovernorateResponse>>> Handle(GetIdentityGovernoratesQuery request, CancellationToken cancellationToken)
    {
        return new CommitResult<List<IdentityGovernorateResponse>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Governorate>().ProjectToType<IdentityGovernorateResponse>().ToListAsync(cancellationToken)
        };
    }
}

