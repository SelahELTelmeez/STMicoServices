﻿using IdentityDomain.Features.ResetPassword.CQRS.Command;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ResetPassword.CQRS.Command;
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IMediator _mediator;

    public ResetPasswordCommandHandler(STIdentityDbContext dbContext, IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(request.ResetPasswordRequest.IdentityUserId), cancellationToken);

        await _dbContext.Set<IdentityUser>().SingleOrDefaultAsync(a => a.Id.Equals(request.ResetPasswordRequest.IdentityUserId), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"],
                ResultType = ResultType.Invalid,
            };
        }
        else
        {
            //2.0 update user password with new password.
            identityUser.PasswordHash = request.ResetPasswordRequest.NewPassword;
            _dbContext.Set<IdentityUser>().Update(identityUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
    }
}