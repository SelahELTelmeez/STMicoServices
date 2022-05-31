using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaymentDomain.Features.TPay.CQRS.Command;
using PaymentDomain.Features.TPay.DTO.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using PaymentInfrastructure.HttpClients;
using SharedModule.Extensions;
using SharedModule.Utilities;

namespace PaymentInfrastructure.Features.TPay.CQRS.Command
{
    public class TPayInitializerCommandHandler : IRequestHandler<TPayInitializerCommand, CommitResult<int>>
    {
        private readonly TPayClient _TPayClient;
        private readonly PaymentDbContext _dbContext;
        private readonly Guid? _userId;
        public TPayInitializerCommandHandler(TPayClient tPayClient, IHttpContextAccessor httpContextAccessor, PaymentDbContext dbContext)
        {
            _TPayClient = tPayClient;
            _userId = httpContextAccessor.GetIdentityUserId();
            _dbContext = dbContext;
        }
        public async Task<CommitResult<int>> Handle(TPayInitializerCommand request, CancellationToken cancellationToken)
        {
            Product? product = await _dbContext.Set<Product>().SingleOrDefaultAsync(a => a.Id.Equals(request.PayInitializerRequest.ProductId), cancellationToken);
            if (product == null)
            {
                return new CommitResult<int>
                {
                    ErrorCode = "X0000",
                    ErrorMessage = "X0000",
                    ResultType = ResultType.NotFound
                };
            }

            CommitResult<TPayInitializerResponse> commitResult = await _TPayClient.InitializerAsync(request.PayInitializerRequest, product, cancellationToken);
            if (!commitResult.IsSuccess)
            {
                return new CommitResult<int>
                {
                    ResultType = ResultType.Invalid,
                    ErrorCode = "X0000",
                    ErrorMessage = CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage)
                };
            }
            if (commitResult.Value.OperationStatusCode == 0) // everything is okay
            {
                EntityEntry<PurchaseContract> purchaseContract = _dbContext.Set<PurchaseContract>().Add(new PurchaseContract
                {
                    UserId = _userId.GetValueOrDefault(),
                    PlanType = "OneTimePayment",
                    TransactionId = commitResult.Value.TransactionId,
                    TransactionStatus = 0,
                    ExpiredOn = DateTime.Today.AddDays(product.SubscriptionDurationInDays),
                    ProductId = request.PayInitializerRequest.ProductId
                });

                await _dbContext.SaveChangesAsync(cancellationToken);

                return new CommitResult<int>
                {
                    ResultType = ResultType.Ok,
                    Value = purchaseContract.Entity.Id,
                };
            }
            else
            {
                return new CommitResult<int>
                {
                    ResultType = ResultType.Invalid,
                    ErrorCode = "X0000",
                    ErrorMessage = CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage)
                };
            }
        }


    }

}
