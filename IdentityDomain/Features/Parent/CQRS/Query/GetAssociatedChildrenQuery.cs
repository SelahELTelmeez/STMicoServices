using ResultHandler;
using SharedModule.DTO;

namespace IdentityDomain.Features.Parent.CQRS.Query;

public record GetAssociatedChildrenQuery : IRequest<CommitResults<LimitedProfileResponse>>;