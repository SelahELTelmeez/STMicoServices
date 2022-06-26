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
public class TPayResendPinCodeCommandHandler : IRequestHandler<TPayResendPinCodeCommand, ICommitResult>
{
    private readonly TPayClient _TPayClient;
    private readonly PaymentDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public TPayResendPinCodeCommandHandler(TPayClient tPayClient, PaymentDbContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
    {
        _TPayClient = tPayClient;
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(httpContextAccessor, webHostEnvironment);
    }

    public async Task<ICommitResult> Handle(TPayResendPinCodeCommand request, CancellationToken cancellationToken)
    {
        PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>().SingleOrDefaultAsync(a => a.Id == request.PurchaseContractId);

        if (purchaseContract == null)
        {
            return ResultType.NotFound.GetCommitResult("XPYM0005", _resourceJsonManager["XPYM0005"]);
        }

        //TODO: 2 send in the paramerts means arabic, 1 means english, we need to make it dynamtic.

        ICommitResult<TPayEndpointResendPinCodeResponse> commitResult = await _TPayClient.ResendPinCodeAsync(purchaseContract.TransactionId, 2, cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return ResultType.Invalid.GetCommitResult(commitResult.ErrorCode, commitResult.ErrorMessage);
        }

        if (commitResult.Value.OperationStatusCode == 0) // everything is okay
        {
            return ResultType.Ok.GetCommitResult();
        }
        else
        {
            return ResultType.Invalid.GetCommitResult("XPYM0000", errorMessage: CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage));
        }
    }
}