using Flaminco.CommitResult;

namespace IdentityDomain.Features.Parent.CQRS.Command;
public record RemoveChildCommand(Guid ChildId) : IRequest<ICommitResult>;

