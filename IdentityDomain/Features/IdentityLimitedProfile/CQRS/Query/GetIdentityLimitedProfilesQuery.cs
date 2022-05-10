using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfilesQuery(IEnumerable<Guid> Ids) : IRequest<CommitResults<LimitedProfileResponse>>;

