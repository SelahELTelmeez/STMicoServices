using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfilesQuery(IEnumerable<string> Ids) : IRequest<ICommitResults<LimitedProfileResponse>>;

