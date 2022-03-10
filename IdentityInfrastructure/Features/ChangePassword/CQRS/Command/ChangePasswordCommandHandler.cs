using IdentityDomain.Features.ChangePassword.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ChangePassword.CQRS.Command;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, CommitResult>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ChangePasswordCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(request.IdentityChangePasswordRequest.IdentityUserId) &&
                                                                                                    a.PasswordHash.Equals(request.IdentityChangePasswordRequest.OldPassword), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Start updating user data in the databse.
            // Add Mapping here.
            identityUser.PasswordHash = request.IdentityChangePasswordRequest.NewPassword;
            _dbContext.Set<IdentityUser>().Add(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}