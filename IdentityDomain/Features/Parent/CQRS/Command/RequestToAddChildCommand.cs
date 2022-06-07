using ResultHandler;

namespace IdentityDomain.Features.Parent.CQRS.Command;

public record RequestToAddChildCommand(Guid ChildId) : IRequest<CommitResult>;



