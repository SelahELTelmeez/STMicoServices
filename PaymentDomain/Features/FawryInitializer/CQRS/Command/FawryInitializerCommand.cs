using PaymentDomain.Features.FawryInitializer.DTO.Command;

namespace PaymentDomain.Features.FawryInitializer.CQRS.Command;

public record FawryInitializerCommand(FawryInitializerRequest FawryInitializerRequest) : IRequest<CommitResult<FawryInitializerRespons>>;


