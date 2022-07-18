namespace PaymentDomain.Features.Contract.Query;
public record ValidateCurrentPurchaseContractQuery(string? UserId) : IRequest<ICommitResult<bool>>;

