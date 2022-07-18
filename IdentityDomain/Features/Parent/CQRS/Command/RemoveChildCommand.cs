using Flaminco.CommitResult;

namespace IdentityDomain.Features.Parent.CQRS.Command;
public record RemoveChildCommand(string ChildId) : IRequest<ICommitResult>;

