namespace PaymentDomain.Features.Contract.Query;
public record ValidateCurrentPurchaseContractQuery(Guid? UserId) : IRequest<ICommitResult<bool>>;

