using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfileQuery(Guid Id) : IRequest<ICommitResult<LimitedProfileResponse>>;


