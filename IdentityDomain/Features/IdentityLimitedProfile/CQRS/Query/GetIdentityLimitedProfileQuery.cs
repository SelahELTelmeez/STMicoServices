using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfileQuery(Guid Id) : IRequest<CommitResult<LimitedProfileResponse>>;


