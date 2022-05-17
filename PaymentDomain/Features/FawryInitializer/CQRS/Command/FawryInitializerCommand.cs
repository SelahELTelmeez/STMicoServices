namespace PaymentDomain.Features.FawryInitializer.CQRS.Command;

public record FawryInitializerCommand(int? Grade, int? ProductId) : IRequest<CommitResult<Guid>>;


