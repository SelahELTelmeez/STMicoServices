using Flaminco.CommitResult;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaymentDomain.Features.Contract.CQRS.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.Contract.CQRS.Command;

public class UnsubscribeAccountCommandHandler : IRequestHandler<UnsubscribeAccountCommand, ICommitResult>
{
    private readonly PaymentDbContext _dbContext;
    private readonly Guid? _userId;
    public UnsubscribeAccountCommandHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<ICommitResult> Handle(UnsubscribeAccountCommand request, CancellationToken cancellationToken)
    {
        PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>().SingleOrDefaultAsync(a => a.UserId == _userId && DateTime.UtcNow.InRange(a.CreatedOn.GetValueOrDefault(), a.ExpiredOn), cancellationToken);
        if (purchaseContract == null)
        {
            return Flaminco.CommitResult.ResultType.Empty.GetCommitResult("", "The current user doesn't have a valid subscription already!");
        }
        else
        {
            _dbContext.Set<PurchaseContract>().Remove(purchaseContract);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return Flaminco.CommitResult.ResultType.Ok.GetCommitResult();
    }
}


