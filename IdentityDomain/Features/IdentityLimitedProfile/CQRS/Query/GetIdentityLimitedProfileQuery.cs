using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfileQuery(Guid Id) : IRequest<CommitResult<LimitedProfileResponse>>;


