using IdentityDomain.Features.Shared.RemoveOldOTP.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Shared.RemoveOldOTP.CQRS.Command;
public class RemoveOldOTPCommandHandler : IRequestHandler<RemoveOldOTPCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;

    public RemoveOldOTPCommandHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult> Handle(RemoveOldOTPCommand request, CancellationToken cancellationToken)
    {
        /// Remove Old OTP
        List<IdentityActivation>? identityActivations = await _dbContext.Set<IdentityActivation>()
                                                                        .Where(a => a.IdentityUserId.Equals(request.IdentityUserId))
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