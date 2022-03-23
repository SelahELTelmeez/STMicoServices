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
                                      a.Code.Equals(request.EmailVerificationRequest.Code) &&
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
                //2.0 Start updating user data in the databse.
                identityActivation.IsVerified = true;
                _dbContext.Set<IdentityActivation>().Update(identityActivation);
                await _dbContext.SaveChangesAsync(cancellationToken);

                /// Remove Old OTP
                List<IdentityActivation>? identityActivations = await _dbContext.Set<IdentityActivation>()
                .Where(a => a.IdentityUserId.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)) && a.CreatedOn.Hour > 24)
                .ToListAsync(cancellationToken);

                _dbContext.Set<IdentityActivation>().Remove(identityActivation);
                _dbContext.SaveChangesAsync(cancellationToken);

                return new CommitResult
                {
                    ResultType = ResultType.Ok
                };
            }
        }
    }
}
