using Flaminco.CommitResult;
using Microsoft.EntityFrameworkCore;
using PaymentDomain.Features.TPay.CQRS.Command;
using PaymentDomain.Features.TPay.DTO.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using PaymentInfrastructure.HttpClients;
using SharedModule.Utilities;

namespace PaymentInfrastructure.Features.TPay.CQRS.Command;
public class TPayConfirmPaymentCommandHandler : IRequestHandler<TPayConfirmPaymentCommand, ICommitResult<TPayConfirmPaymentResponse>>
{
    private readonly TPayClient _TPayClient;
    private readonly PaymentDbContext _dbContext;
    public TPayConfirmPaymentCommandHandler(TPayClient tPayClient, PaymentDbContext dbContext)
    {
        _TPayClient = tPayClient;
        _dbContext = dbContext;
    }
    public async Task<ICommitResult<TPayConfirmPaymentResponse>> Handle(TPayConfirmPaymentCommand request, CancellationToken cancellationToken)
    {

        PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>().SingleOrDefaultAsync(a => a.Id == request.TPayConfirmPaymentRequest.PurchaseContractId);

        if (purchaseContract == null)
        {
            return ResultType.NotFound.GetValueCommitResult<TPayConfirmPaymentResponse>(default, "X0000");
        }

        ICommitResult<TPayEndpointConfirmPaymentResponse> commitResult = await _TPayClient.ConfirmPaymentAsync(request.TPayConfirmPaymentRequest.PinCode, purchaseContract.TransactionId, cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return ResultType.Invalid.GetValueCommitResult<TPayConfirmPaymentResponse>(default, "X0000", errorMessage: "Couldn't make the connection to TPay Server");
        }

        if (commitResult.Value.OperationStatusCode == 0) // everything is okay
        {
            purchaseContract.TransactionStatus = 2;
            purchaseContract.CallbackOn = DateTime.UtcNow;

            _dbContext.Set<PurchaseContract>().Update(purchaseContract);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetValueCommitResult(new TPayConfirmPaymentResponse
            {
                AmountCharged = commitResult.Value.AmountCharged,
                CurrencyCode = commitResult.Value.CurrencyCode
            });
        }
        else
        {
            return ResultType.Invalid.GetValueCommitResult<TPayConfirmPaymentResponse>(default, "X0000", errorMessage: CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage));
        }
    }
}