using PaymentDomain.Features.TPay.DTO.Command;

namespace PaymentDomain.Features.TPay.CQRS.Command;

public record TPayConfirmPaymentCommand(TPayConfirmPaymentRequest TPayConfirmPaymentRequest) : IRequest<CommitResult<TPayConfirmPaymentResponse>>;

