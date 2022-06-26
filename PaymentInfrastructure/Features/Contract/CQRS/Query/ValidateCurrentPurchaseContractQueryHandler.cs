using Flaminco.CommitResult;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaymentDomain.Features.Contract.Query;
using PaymentEntities;
using PaymentEntities.Entities;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.Contract.CQRS.Query;

public class ValidateCurrentPurchaseContractQueryHandler : IRequestHandler<ValidateCurrentPurchaseContractQuery, ICommitResult<bool>>
{
    private readonly PaymentDbContext _dbContext;
    private readonly Guid? _userId;

    public ValidateCurrentPurchaseContractQueryHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<ICommitResult<bool>> Handle(ValidateCurrentPurchaseContractQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResult<bool>(await _dbContext.Set<PurchaseContract>().Where(a => a.UserId.Equals(_userId) && a.TransactionStatus > 1 && a.ExpiredOn >= DateTime.Today).AnyAsync(cancellationToken));

    }
}


