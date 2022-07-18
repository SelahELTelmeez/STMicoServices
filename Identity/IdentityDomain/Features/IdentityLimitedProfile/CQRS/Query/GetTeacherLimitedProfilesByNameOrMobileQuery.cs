using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetTeacherLimitedProfilesByNameOrMobileQuery(string NameOrMobileNumber) : IRequest<ICommitResults<LimitedProfileResponse>>;

