using IdentityDomain.Features.EmailVerification.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.EmailVerification.CQRS.Command
{
    public class EmailVerificationCommandHandler : IRequestHandler<EmailVerificationCommand, CommitResult>
    {
        private readonly AuthenticationDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmailVerificationCommandHandler(AuthenticationDbContext dbContext, JsonLocalizerManager resourceJsonManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = resourceJsonManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommitResult> Handle(EmailVerificationCommand request, CancellationToken cancellationToken)
        {
            // 1.0 Check for the user Id existance first, with the provided data.
            IdentityActivation? identityActivation = await _dbContext.Set<IdentityActivation>()
                .SingleOrDefaultAsync(a => a.IdentityUserId.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)) &&
                                      a.Code.Equals(request.EmailVerificationRequest.Code) &&
                                      a.ActivationType.Equals(ActivationType.Email), cancellationToken);

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
}
