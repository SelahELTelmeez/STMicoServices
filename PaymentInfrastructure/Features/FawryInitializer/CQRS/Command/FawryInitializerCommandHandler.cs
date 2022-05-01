using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentDomain.Features.FawryInitializer.CQRS.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.FawryInitializer.CQRS.Command;

public class FawryInitializerCommandHandler : IRequestHandler<FawryInitializerCommand, CommitResult<Guid>>
{
    private readonly PaymentDbContext _dbContext;
    private readonly Guid? _userId;
    public FawryInitializerCommandHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }


    public async Task<CommitResult<Guid>> Handle(FawryInitializerCommand request, CancellationToken cancellationToken)
    {
        Product? product = await _dbContext.Set<Product>().SingleOrDefaultAsync(a => a.Id.Equals(request.FawryInitializerRequest.ProductId), cancellationToken);
        if (product == null)
        {
            return new CommitResult<Guid>
            {
                ErrorCode = "X0000",
                ErrorMessage = "X0000",
                ResultType = ResultType.NotFound
            };
        }

        EntityEntry<PurchaseContract> purchaseContract = _dbContext.Set<PurchaseContract>().Add(new PurchaseContract
        {
            UserId = _userId.GetValueOrDefault(),
            PlanType = "FawryPayment",
            TransactionId = Guid.NewGuid(),
            TransactionStatus = 0,
            ExpiredOn = DateTime.Today.AddDays(product.SubscriptionDurationInDays),
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult<Guid>
        {
            ResultType = ResultType.Ok,
            Value = purchaseContract.Entity.TransactionId
        };
    }
}
