using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfilesQuery(IEnumerable<Guid> Ids) : IRequest<CommitResults<LimitedProfileResponse>>;

