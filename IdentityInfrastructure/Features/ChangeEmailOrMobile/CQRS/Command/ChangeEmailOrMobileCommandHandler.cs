using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ChangeEmailOrMobile.CQRS.Command;
public class ChangeEmailOrMobileCommandHandler : IRequestHandler<ChangeEmailOrMobileCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangeEmailOrMobileCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(ChangeEmailOrMobileCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(HttpIdentityUser.GetIdentityUserId(_httpContextAccessor)) &&
                                                                                                    a.PasswordHash.Equals(request.ChangeEmailOrMobileRequest.Password), cancellationToken);

        if (identityUser == null)
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
            identityUser.Email = request.ChangeEmailOrMobileRequest.NewEmail;
            identityUser.MobileNumber = request.ChangeEmailOrMobileRequest.NewMobileNumber;
            identityUser.PasswordHash = request.ChangeEmailOrMobileRequest.Password;
            _dbContext.Set<IdentityUser>().Add(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}