using Microsoft.EntityFrameworkCore;
using PaymentDomain.Features.TPay.CQRS.Command;
using PaymentDomain.Features.TPay.DTO.Command;
using PaymentEntities;
using PaymentEntities.Entities;
using PaymentInfrastructure.HttpClients;
using SharedModule.Utilities;

namespace PaymentInfrastructure.Features.TPay.CQRS.Command
{
    public class TPayResendPinCodeCommandHandler : IRequestHandler<TPayResendPinCodeCommand, CommitResult>
    {
        private readonly TPayClient _TPayClient;
        private readonly PaymentDbContext _dbContext;
        public TPayResendPinCodeCommandHandler(TPayClient tPayClient, PaymentDbContext dbContext)
        {
            _TPayClient = tPayClient;
            _dbContext = dbContext;
        }

        public async Task<CommitResult> Handle(TPayResendPinCodeCommand request, CancellationToken cancellationToken)
        {
            PurchaseContract? purchaseContract = await _dbContext.Set<PurchaseContract>().SingleOrDefaultAsync(a => a.Id == request.PurchaseContractId);

            if (purchaseContract == null)
            {
                return new CommitResult
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "X000",
                    ErrorMessage = "X00000"
                };
            }

            //TODO: 2 send in the paramerts means arabic, 1 means english, we need to make it dynamtic.

            CommitResult<TPayEndpointResendPinCodeResponse> commitResult = await _TPayClient.ResendPinCodeAsync(purchaseContract.TransactionId, 2, cancellationToken);

            if (!commitResult.IsSuccess)
            {
                return new CommitResult<TPayConfirmPaymentResponse>
                {
                    ResultType = ResultType.Invalid,
                    ErrorCode = "X0000",
                    ErrorMessage = "Couldn't make the connection to TPay Server"
                };
            }

            if (commitResult.Value.OperationStatusCode == 10) // everything is okay
            {
                return new CommitResult
                {
                    ResultType = ResultType.Ok,
                };
            }
            else
            {
                return new CommitResult<TPayConfirmPaymentResponse>
                {
                    ResultType = ResultType.Invalid,
                    ErrorCode = "X0000",
                    ErrorMessage = CodeMappers.TPayCodeMapper(commitResult.Value.OperationStatusCode, commitResult.Value.ErrorMessage)
                };
            }
        }
    }
}
