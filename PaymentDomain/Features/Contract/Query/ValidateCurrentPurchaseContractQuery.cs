namespace PaymentDomain.Features.Contract.Query;
public record ValidateCurrentPurchaseContractQuery : IRequest<ICommitResult<bool>>;

