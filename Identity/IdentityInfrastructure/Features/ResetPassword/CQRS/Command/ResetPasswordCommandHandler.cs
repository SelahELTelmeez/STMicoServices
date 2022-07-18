using Flaminco.CommitResult;
using IdentityDomain.Features.ResetPassword.CQRS.Command;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace IdentityInfrastructure.Features.ResetPassword.CQRS.Command;
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ICommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IMediator _mediator;

    public ResetPasswordCommandHandler(STIdentityDbContext dbContext, IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _mediator = mediator;
    }

    public async Task<ICommitResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(request.ResetPasswordRequest.IdentityUserId), cancellationToken);

        if (identityUser == null)
        {
            return ResultType.Invalid.GetCommitResult("XIDN0001", _resourceJsonManager["XIDN0001"]);
        }
        else
        {
            //2.0 update user password with new password.
            identityUser.PasswordHash = request.ResetPasswordRequest.NewPassword.Encrypt(true);
            _dbContext.Set<IdentityUser>().Update(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetCommitResult();
        }
    }
}