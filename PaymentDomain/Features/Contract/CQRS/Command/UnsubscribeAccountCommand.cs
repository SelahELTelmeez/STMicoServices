namespace PaymentDomain.Features.Contract.CQRS.Command;

public record UnsubscribeAccountCommand() : IRequest<ICommitResult>;


