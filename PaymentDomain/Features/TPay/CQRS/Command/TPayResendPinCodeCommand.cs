namespace PaymentDomain.Features.TPay.CQRS.Command;

public record TPayResendPinCodeCommand(int PurchaseContractId) : IRequest<ICommitResult>;
