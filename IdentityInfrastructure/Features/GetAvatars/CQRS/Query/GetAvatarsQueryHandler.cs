using IdentityDomain.Features.GetAvatars.CQRS.Query;
using IdentityDomain.Features.GetAvatars.DTO.Query;
using IdentityEntities.Entities;
using IdentityEntities.Shared.Identities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.GetAvatars.CQRS.Query;

public class GetAvatarsQueryHandler : IRequestHandler<GetAvatarsQuery, CommitResult<List<AvatarResponseDTO>>>
{
    private readonly STIdentityDbContext _dbContext;
    public GetAvatarsQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult<List<AvatarResponseDTO>>> Handle(GetAvatarsQuery request, CancellationToken cancellationToken)
    {
        return new CommitResult<List<AvatarResponseDTO>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Avatar>().ProjectToType<AvatarResponseDTO>().ToListAsync(cancellationToken)
        };
    }
}
