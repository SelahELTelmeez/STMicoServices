using IdentityDomain.Features.Shared.DTO;
using ResultHandler;

namespace IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;

public record GetIdentityRelationUserQuery : IRequest<CommitResults<LimitedProfileResponse>>;