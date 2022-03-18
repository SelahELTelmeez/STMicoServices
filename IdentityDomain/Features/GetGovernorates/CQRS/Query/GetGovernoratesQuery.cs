using IdentityDomain.Features.GetGovernorates.DTO;
using ResultHandler;

namespace IdentityDomain.Features.GetGovernorates.CQRS.Query;

public record GetGovernoratesQuery() : IRequest<CommitResult<List<GovernorateResponseDTO>>>;


