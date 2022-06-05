using Flaminco.CommitResult;

namespace NotifierDomain.Features.CQRS.Command;

public record SetAsSeenInvitationCommand() : IRequest<ICommitResult>;


