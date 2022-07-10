using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetIdentityLimitedProfileByEmailOrMobileQuery(string? Email, string? MobileNumber) : IRequest<ICommitResult<LimitedProfileResponse>>;

