using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;

public record GetTeacherLimitedProfilesByNameOrMobileQuery(string NameOrMobileNumber) : IRequest<CommitResults<LimitedProfileResponse>>;

