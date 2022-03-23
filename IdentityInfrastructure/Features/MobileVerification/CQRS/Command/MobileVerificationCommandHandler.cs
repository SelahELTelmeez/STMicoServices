using IdentityDomain.Features.MobileVerification.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
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
    public MobileVerificationCommandHandler(STIdentityDbContext dbContext,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(MobileVerificationCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>()
            .SingleOrDefaultAsync(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                       a.ActivationType == ActivationType.Mobile &&
                                       a.Code.Equals(request.MobileVerificationRequest.Code), cancellationToken);

        if (identityActivation == null || (!identityActivation.IsActive))
        {
            return new CommitResult
            {
                ErrorCode = "X0004",
                ErrorMessage = _resourceJsonManager["X0004"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Start updating user data in the databse.
            await _dbContext.Entry(identityActivation).Reference(a => a.IdentityUserFK).LoadAsync(cancellationToken);
            identityActivation.IdentityUserFK.IsVerified = true;
            //2.0 Start updating user data in the databse.
            identityActivation.IsVerified = true;

            await _dbContext.SaveChangesAsync(cancellationToken);

            /// Remove Old OTP
            List<IdentityActivation>? identityActivations = await _dbContext.Set<IdentityActivation>()
            .Where(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()))
            .ToListAsync(cancellationToken);

            _dbContext.Set<IdentityActivation>().RemoveRange(identityActivations.Where(a => (DateTime.UtcNow.Subtract(a.CreatedOn)).TotalHours > 24));
            _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}