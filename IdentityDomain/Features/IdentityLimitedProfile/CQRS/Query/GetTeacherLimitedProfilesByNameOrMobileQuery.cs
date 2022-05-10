using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetTeacherLimitedProfilesByNameOrMobileQuery(string NameOrMobileNumber) : IRequest<CommitResults<LimitedProfileResponse>>;

