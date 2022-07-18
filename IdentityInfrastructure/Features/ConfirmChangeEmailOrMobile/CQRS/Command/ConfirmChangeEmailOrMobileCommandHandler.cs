using Flaminco.CommitResult;
using IdentityDomain.Features.ConfirmChangeEmailOrMobile.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.ConfirmChangeEmailOrMobile.CQRS.Command;

public class ConfirmChangeEmailOrMobileCommandHandler : IRequestHandler<ConfirmChangeEmailOrMobileCommand, ICommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly string? _userId;
    public ConfirmChangeEmailOrMobileCommandHandler(STIdentityDbContext dbContext,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<ICommitResult> Handle(ConfirmChangeEmailOrMobileCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.

        IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>().FirstOrDefaultAsync(a => a.Code == request.OTPVerificationRequest.Code, cancellationToken);


        if (identityActivation == null || identityActivation.IsActive == false)
        {
            return ResultType.Invalid.GetCommitResult("XIDN0004", _resourceJsonManager["XIDN0004"]);
        }
        else
        {
            //2.0 update user password with new password.
            identityActivation.IsVerified = true;
            _dbContext.Set<IdentityActivation>().Update(identityActivation);

            IdentityTemporaryValueHolder? identityTemporaryValueHolder = await _dbContext.Set<IdentityTemporaryValueHolder>()
                .FirstOrDefaultAsync(a => a.IdentityUserId.Equals(identityActivation.IdentityUserId) && a.Name.Equals(identityActivation.ActivationType.ToString()), cancellationToken);

            if (identityTemporaryValueHolder == null)
            {
                return ResultType.Invalid.GetCommitResult("XIDN0015", _resourceJsonManager["XIDN0015"]);
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
            // Remove All other temp values for this current user

            IEnumerable<IdentityTemporaryValueHolder> valueHolders = await _dbContext.Set<IdentityTemporaryValueHolder>().Where(a => a.IdentityUserId == _userId).ToListAsync(cancellationToken);

            if (valueHolders.Any())
            {
                _dbContext.Set<IdentityTemporaryValueHolder>().RemoveRange(valueHolders);
            }

            _dbContext.Set<IdentityUser>().Update(identityActivation.IdentityUserFK);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetCommitResult();
        }
    }
}

