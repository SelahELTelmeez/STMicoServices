using IdentityDomain.Features.IdentityGovernorates.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityGovernorates.CQRS.Query;

public record GetIdentityGovernoratesQuery() : IRequest<CommitResults<IdentityGovernorateResponse>>;


