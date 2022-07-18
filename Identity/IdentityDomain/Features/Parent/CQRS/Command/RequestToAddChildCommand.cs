using Flaminco.CommitResult;

namespace IdentityDomain.Features.Parent.CQRS.Command;

public record RequestToAddChildCommand(string ChildId) : IRequest<ICommitResult>;



