using IdentityDomain.Features.MobileVerification.CQRS.Command;
using IdentityDomain.Features.Shared.RemoveOldOTP.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.MobileVerification.CQRS.Command;
public class MobileVerificationCommandHandler : IRequestHandler<MobileVerificationCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public MobileVerificationCommandHandler(STIdentityDbContext dbContext,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor,
                                            IMediator mediator)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
    }

    public async Task<CommitResult> Handle(MobileVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>()
            .SingleOrDefaultAsync(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                       a.ActivationType == ActivationType.Mobile &&
                                       a.Code.Equals(request.OTPVerificationRequest.Code), cancellationToken);

        if (identityActivation == null || (!identityActivation.IsActive))
        {
            return new CommitResult
            {
                ErrorCode = "X0004",
                ErrorMessage = _resourceJsonManager["X0004"],
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Start updating user data in the databse.
            await _dbContext.Entry(identityActivation).Reference(a => a.IdentityUserFK).LoadAsync(cancellationToken);
            identityActivation.IdentityUserFK.IsMobileVerified = true;
            //2.0 Start updating user data in the databse.
            identityActivation.IsVerified = true;

            await _dbContext.SaveChangesAsync(cancellationToken);

            /// Remove Old OTP
            return await _mediator.Send(new RemoveOldOTPCommand(_httpContextAccessor.GetIdentityUserId()), cancellationToken);
        }
    }
}