using Flaminco.CommitResult;
using Flaminco.JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    private readonly JsonLocalizerManager _resourceJsonManager;
    public TPayConfirmPaymentCommandHandler(TPayClient tPayClient, PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
    {
        _TPayClient = tPayClient;
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, webHostEnvironment);
    }
    public async Task<ICommitResult<TPayConfirmPaymentResponse>> Handle(TPayConfirmPaymentCommand request, CancellationToken cancellationToken)
    {

        PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>().FirstOrDefaultAsync(a => a.Id == request.TPayConfirmPaymentRequest.PurchaseContractId);

        if (purchaseContract == null)
        {
            return ResultType.NotFound.GetValueCommitResult<TPayConfirmPaymentResponse>(default, "XPYM0005", _resourceJsonManager["XPYM0005"]);
        }

        ICommitResult<TPayEndpointConfirmPaymentResponse> commitResult = await _TPayClient.ConfirmPaymentAsync(request.TPayConfirmPaymentRequest.PinCode, purchaseContract.TransactionId, cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return ResultType.Invalid.GetValueCommitResult<TPayConfirmPaymentResponse>(default, commitResult.ErrorCode, commitResult.ErrorMessage);
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
            return ResultType.Invalid.GetValueCommitResult<TPayConfirmPaymentResponse>(default, "XPYM0000", errorMessage: CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage));
        }
    }
}