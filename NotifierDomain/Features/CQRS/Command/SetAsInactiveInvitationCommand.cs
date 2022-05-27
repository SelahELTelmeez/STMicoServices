using Flaminco.CommitResult;

namespace NotifierDomain.Features.CQRS.Command;

public record SetAsInactiveInvitationCommand(int InvitationId) : IRequest<ICommitResult>;

