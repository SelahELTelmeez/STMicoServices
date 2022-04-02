using IdentityDomain.Features.EmailVerification.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.EmailVerification.CQRS.Command
{
    public class EmailVerificationCommandHandler : IRequestHandler<EmailVerificationCommand, CommitResult>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmailVerificationCommandHandler(STIdentityDbContext dbContext,
                                               IWebHostEnvironment configuration,
                                               IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommitResult> Handle(EmailVerificationCommand request, CancellationToken cancellationToken)
        {
            // 1.0 Check for the user Id existance first, with the provided data.
            IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>()
                .SingleOrDefaultAsync(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                      a.Code.Equals(request.OTPVerificationRequest.Code) &&
                                      a.ActivationType == ActivationType.Email, cancellationToken);

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
                await _dbContext.Entry(identityActivation).Reference(a => a.IdentityUserFK).LoadAsync(cancellationToken);
                identityActivation.IdentityUserFK.IsEmailVerified = true;
                //2.0 Start updating user data in the databse.
                identityActivation.IsVerified = true;

                await _dbContext.SaveChangesAsync(cancellationToken);

                /// Remove Old OTP
                List<IdentityActivation>? identityActivations = await _dbContext.Set<IdentityActivation>()
                                                                                .Where(a => a.IdentityUserId.Equals(_httpContextAccessor.GetIdentityUserId()))
                                                                                .ToListAsync(cancellationToken);

                if (identityActivations.Any(a => (DateTime.UtcNow.Subtract(a.CreatedOn)).TotalHours > 24))
                {
                    _dbContext.Set<IdentityActivation>().RemoveRange(identityActivations.Where(a => (DateTime.UtcNow.Subtract(a.CreatedOn)).TotalHours > 24));
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return new CommitResult
                {
                    ResultType = ResultType.Ok
                };
            }
        }
    }
}
