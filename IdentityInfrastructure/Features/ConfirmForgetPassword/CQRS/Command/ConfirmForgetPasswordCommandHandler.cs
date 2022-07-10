using Flaminco.CommitResult;
using IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.ConfirmForgetPassword.CQRS.Command;
public class ConfirmForgetPasswordCommandHandler : IRequestHandler<ConfirmForgetPasswordCommand, ICommitResult<Guid>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ConfirmForgetPasswordCommandHandler(STIdentityDbContext dbContext,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult<Guid>> Handle(ConfirmForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.

        IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>().FirstOrDefaultAsync(a => a.Code == request.OTPVerificationRequest.Code, cancellationToken);

        if (identityActivation == null || identityActivation.IsActive == false)
        {
            return ResultType.Invalid.GetValueCommitResult<Guid>(Guid.Empty,"XIDN0004", _resourceJsonManager["XIDN0004"]);
        }
        else
        {
            //2.0 update user password with new password.
            identityActivation.IsVerified = true;
            _dbContext.Set<IdentityActivation>().Update(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetValueCommitResult(identityActivation.IdentityUserId);
        }
    }
}