using Flaminco.CommitResult;
using IdentityDomain.Features.ChangePassword.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.ChangePassword.CQRS.Command;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ICommitResult>
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

    public async Task<ICommitResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _dbContext.Set<IdentityUser>().FirstOrDefaultAsync(a => a.Id.Equals(_httpContextAccessor.GetIdentityUserId()) &&
                                                                                                    a.PasswordHash.Equals(request.ChangePasswordRequest.OldPassword.Encrypt(true)), cancellationToken);

        if (identityUser == null)
        {
            return ResultType.NotFound.GetCommitResult("XIDN0001", _resourceJsonManager["XIDN0001"]);
        }
        else
        {
            //2.0 Start updating user data in the databse.
            // Add Mapping here.
            identityUser.PasswordHash = request.ChangePasswordRequest.NewPassword.Encrypt(true);
            _dbContext.Set<IdentityUser>().Update(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetCommitResult();
        }
    }
}