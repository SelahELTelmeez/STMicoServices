using PaymentDomain.Features.TPay.DTO.Command;

namespace PaymentDomain.Features.TPay.CQRS.Command;

public record TPayInitializerCommand(TPayInitializerRequest PayInitializerRequest) : IRequest<CommitResult<int>>;

