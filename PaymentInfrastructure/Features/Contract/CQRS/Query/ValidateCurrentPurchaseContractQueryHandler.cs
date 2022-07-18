using Flaminco.CommitResult;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PaymentDomain.Features.Contract.Query;
using PaymentEntities;
using PaymentEntities.Entities;
using PaymentInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace PaymentInfrastructure.Features.Contract.CQRS.Query;

public class ValidateCurrentPurchaseContractQueryHandler : IRequestHandler<ValidateCurrentPurchaseContractQuery, ICommitResult<bool>>
{
    private readonly PaymentDbContext _dbContext;
    private readonly string? _userId;
    private readonly IdentityClient _identityClient;
    public ValidateCurrentPurchaseContractQueryHandler(PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }

    public async Task<ICommitResult<bool>> Handle(ValidateCurrentPurchaseContractQuery request, CancellationToken cancellationToken)
    {
        ICommitResult<LimitedProfileResponse>? limitedProfileResponse = await _identityClient.GetIdentityLimitedProfileAsync(request.UserId ?? _userId, cancellationToken);

        if (!limitedProfileResponse?.IsSuccess ?? true)
        {
            return ResultType.Ok.GetValueCommitResult(false);
        }

        if (limitedProfileResponse?.Value?.IsPremium ?? false)
        {
            return ResultType.Ok.GetValueCommitResult(true);
        }

        PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>()
                                                             .Include(a => a.ProductFK)
                                                             .FirstOrDefaultAsync(a => a.UserId.Equals(request.UserId ?? _userId) && a.TransactionStatus > 1 && a.ExpiredOn >= DateTime.Today, cancellationToken);
        if (purchaseContract == null)
        {
            return ResultType.Ok.GetValueCommitResult(false);
        }
        else if (purchaseContract.ProductFK.Grade == limitedProfileResponse?.Value?.GradeId)
        {
            return ResultType.Ok.GetValueCommitResult(true);
        }
        else
        {
            return ResultType.Ok.GetValueCommitResult(false);
        }
    }
}


