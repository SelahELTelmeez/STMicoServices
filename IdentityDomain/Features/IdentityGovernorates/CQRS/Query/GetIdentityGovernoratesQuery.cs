using Flaminco.CommitResult;
using IdentityDomain.Features.IdentityGovernorates.DTO;

namespace IdentityDomain.Features.IdentityGovernorates.CQRS.Query;

public record GetIdentityGovernoratesQuery() : IRequest<ICommitResults<IdentityGovernorateResponse>>;


