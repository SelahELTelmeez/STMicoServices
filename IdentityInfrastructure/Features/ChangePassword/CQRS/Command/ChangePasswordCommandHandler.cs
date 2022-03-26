using IdentityDomain.Features.ChangePassword.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ChangePassword.CQRS.Command;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangePasswordCommandHandler(STIdentityDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment configuration)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CommitResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                                                                                    a.PasswordHash.Equals(request.ChangePasswordRequest.OldPassword), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"],
                ResultType = ResultType.NotFound,
            };
        }
        else
        {
            //2.0 Start updating user data in the databse.
            // Add Mapping here.
            identityUser.PasswordHash = request.ChangePasswordRequest.NewPassword;
            _dbContext.Set<IdentityUser>().Update(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}