using Flaminco.CommitResult;
using SharedModule.DTO;

namespace IdentityDomain.Features.Parent.CQRS.Query;

public record GetAssociatedChildrenQuery : IRequest<ICommitResults<LimitedProfileResponse>>;