using IdentityDomain.Features.ValidateToken.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ValidateToken.CQRS.Command;

public class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, CommitResult<bool>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly Guid? _userId;
    public ValidateTokenCommandHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();

    }
    public async Task<CommitResult<bool>> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
    {
        if (_userId == null)
        {
            return new CommitResult<bool>
            {
                ResultType = ResultType.Unauthorized,
                Value = false,
                ErrorCode = "XIDN00021",
                ErrorMessage = "Can't read the user information from the access token"
            };
        }

        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.Id == _userId, cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<bool>
            {
                ResultType = ResultType.Unauthorized,
                Value = false,
                ErrorCode = "XIDN00022",
                ErrorMessage = "The user is not found"
            };
        }
        return new CommitResult<bool>
        {
            ResultType = ResultType.Ok,
            Value = true
        };
    }
}
