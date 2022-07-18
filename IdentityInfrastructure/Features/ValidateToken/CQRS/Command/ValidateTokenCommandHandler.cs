using Flaminco.CommitResult;
using IdentityDomain.Features.ValidateToken.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.ValidateToken.CQRS.Command;

public class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, ICommitResult<bool>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly string? _userId;
    public ValidateTokenCommandHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();

    }
    public async Task<ICommitResult<bool>> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
    {
        if (_userId == null)
        {
            return ResultType.Unauthorized.GetValueCommitResult(false, "XIDN00021", "Can't read the user information from the access token");
        }

        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.Id == _userId, cancellationToken);

        if (identityUser == null)
        {
            return ResultType.Unauthorized.GetValueCommitResult(false, "XIDN00022", "The user is not found");
        }
        return ResultType.Ok.GetValueCommitResult(true);
    }
}
