namespace PaymentDomain.Features.FawryInitializer.CQRS.Command;

public record FawryInitializerCommand(int ProductId) : IRequest<CommitResult<string>>;


