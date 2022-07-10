using Flaminco.CommitResult;

namespace IdentityDomain.Features.Parent.CQRS.Command;

public record RequestToAddChildCommand(Guid ChildId) : IRequest<ICommitResult>;



