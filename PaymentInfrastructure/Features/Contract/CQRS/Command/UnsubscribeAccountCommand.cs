using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
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
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UnsubscribeAccountCommandHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = jsonLocalizerManager;
    }
    public async Task<ICommitResult> Handle(UnsubscribeAccountCommand request, CancellationToken cancellationToken)
    {
        PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>().SingleOrDefaultAsync(a => a.UserId == _userId && DateTime.UtcNow.InRange(a.CreatedOn.GetValueOrDefault(), a.ExpiredOn), cancellationToken);
        if (purchaseContract == null)
        {
            return ResultType.Empty.GetCommitResult("X0001", _resourceJsonManager["X0001"]);
        }
        else
        {
            _dbContext.Set<PurchaseContract>().Remove(purchaseContract);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return ResultType.Ok.GetCommitResult();
    }
}


