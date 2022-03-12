using IdentityDomain.Features.ResetPassword.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ResetPassword.CQRS.Command;
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ResetPasswordCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(request.ResetPasswordRequest.IdentityUserId), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"],
                ResultType = ResultType.Invalid,
            };
        }
        else
        {
            //2.0 update user password with new password.
            identityUser.PasswordHash = request.ResetPasswordRequest.NewPassword;
            _dbContext.Set<IdentityUser>().Update(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}