using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfileQuery(string Id) : IRequest<ICommitResult<LimitedProfileResponse>>;


