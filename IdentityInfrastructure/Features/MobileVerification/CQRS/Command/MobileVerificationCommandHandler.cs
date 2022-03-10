using IdentityDomain.Features.MobileVerification.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.MobileVerification.CQRS.Command;
public class MobileVerificationCommandHandler : IRequestHandler<MobileVerificationCommand, CommitResult>
{
    private readonly AuthenticationDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public MobileVerificationCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult> Handle(MobileVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>().SingleOrDefaultAsync(a => a.Id.Equals(request.MobileVerificationRequest.IdentityUserId) &&
                                                                                                    a.Code.Equals(request.MobileVerificationRequest.Code) &&
                                                                                                    a.ActivationType.Equals(ActivationType.Mobile), cancellationToken);

        if (identityActivation == null)
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
            identityActivation.IsVerified = true;
            _dbContext.Set<IdentityActivation>().Update(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}