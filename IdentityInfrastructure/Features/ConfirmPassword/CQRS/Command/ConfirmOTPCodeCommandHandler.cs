using IdentityDomain.Features.ConfirmOTPCode.CQRS.Command;
using IdentityDomain.Services;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ConfirmPassword.CQRS.Command;
public class ConfirmOTPCodeCommandHandler : IRequestHandler<ConfirmOTPCodeCommand, CommitResult<Guid>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ConfirmOTPCodeCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult<Guid>> Handle(ConfirmOTPCodeCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.

        IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>()
            .Include(a=>a.IdentityUserFK)
            .SingleOrDefaultAsync(a => a.Code == request.OTPCode &&
                                  a.IsActive, cancellationToken);

        if (identityActivation == null)
        {
            return new CommitResult<Guid>
            {
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"],
                ResultType = ResultType.Invalid,
            };
        }
        else
        {
            //2.0 update user password with new password.
            identityActivation.IsVerified = true;
            _dbContext.Set<IdentityActivation>().Update(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult<Guid>
            {
                ResultType = ResultType.Ok,
                Value = identityActivation.IdentityUserId
            };
        }
    }
}