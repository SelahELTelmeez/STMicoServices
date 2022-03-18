﻿using IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.ConfirmForgetPassword.CQRS.Command;
public class ConfirmOTPCodeCommandHandler : IRequestHandler<ConfirmForgetPasswordCommand, CommitResult<Guid>>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ConfirmOTPCodeCommandHandler(STIdentityDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult<Guid>> Handle(ConfirmForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.

        IEnumerable<IdentityActivation>? identityActivations = await _dbContext.Set<IdentityActivation>().Where(a => a.Code == request.OTPCode.Code).ToListAsync(cancellationToken);

        IdentityActivation? identityActivation = identityActivations?.SingleOrDefault(a => a.IsActive);

        if (identityActivation == null)
        {
            return new CommitResult<Guid>
            {
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"],
                ResultType = ResultType.Invalid,
            };
        }
        else
        {
            //2.0 update user password with new password.
            identityActivation.IsVerified = true;
            _dbContext.Set<IdentityActivation>().Update(identityActivation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CommitResult<Guid>
            {
                ResultType = ResultType.Ok,
                Value = identityActivation.IdentityUserId
            };
        }
    }
}