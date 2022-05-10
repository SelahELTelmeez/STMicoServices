using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;

public record GetIdentityRelationUserQuery : IRequest<CommitResults<LimitedProfileResponse>>;