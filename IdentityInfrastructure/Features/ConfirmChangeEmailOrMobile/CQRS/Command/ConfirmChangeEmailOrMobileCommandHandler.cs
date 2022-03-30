using IdentityDomain.Features.ConfirmChangeEmailOrMobile.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ConfirmChangeEmailOrMobile.CQRS.Command;

public class ConfirmChangeEmailOrMobileCommandHandler : IRequestHandler<ConfirmChangeEmailOrMobileCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ConfirmChangeEmailOrMobileCommandHandler(STIdentityDbContext dbContext,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult> Handle(ConfirmChangeEmailOrMobileCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.

        IEnumerable<IdentityActivation>? identityActivations = await _dbContext.Set<IdentityActivation>().Where(a => a.Code == request.OTPVerificationRequest.Code).ToListAsync(cancellationToken);

        IdentityActivation? identityActivation = identityActivations?.SingleOrDefault(a => a.IsActive);

        if (identityActivation == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0004",
                ErrorMessage = _resourceJsonManager["X0004"],
                ResultType = ResultType.Invalid,
            };
        }
        else
        {
            //2.0 update user password with new password.
            identityActivation.IsVerified = true;
            _dbContext.Set<IdentityActivation>().Update(identityActivation);

            IdentityTemporaryValueHolder? identityTemporaryValueHolder = await _dbContext.Set<IdentityTemporaryValueHolder>().FirstOrDefaultAsync(a => a.IdentityUserId.Equals(identityActivation.IdentityUserId) && a.Name.Equals(identityActivation.ActivationType.ToString()), cancellationToken);

            if (identityTemporaryValueHolder == null)
            {
                return new CommitResult
                {
                    ErrorCode = "X0015",
                    ErrorMessage = _resourceJsonManager["X0015"],
                    ResultType = ResultType.Invalid,
                };
            }

            await _dbContext.Entry(identityActivation).Reference(a => a.IdentityUserFK).LoadAsync(cancellationToken);

            if (identityActivation.ActivationType == ActivationType.Email)
            {
                identityActivation.IdentityUserFK.Email = identityTemporaryValueHolder.Value;
                identityActivation.IdentityUserFK.IsEmailVerified = true;
            }
            else
            {
                identityActivation.IdentityUserFK.MobileNumber = identityTemporaryValueHolder.Value;
                identityActivation.IdentityUserFK.IsMobileVerified = true;
            }
            _dbContext.Set<IdentityTemporaryValueHolder>().Remove(identityTemporaryValueHolder);
            _dbContext.Set<IdentityUser>().Update(identityActivation.IdentityUserFK);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult
            {
                ResultType = ResultType.Ok,
            };
        }
    }
}

