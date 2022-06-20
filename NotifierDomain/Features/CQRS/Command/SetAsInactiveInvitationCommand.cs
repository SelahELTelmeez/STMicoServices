namespace NotifierDomain.Features.CQRS.Command;

public record SetAsInactiveInvitationCommand(int InvitationId, int? Status) : IRequest<ICommitResult>;

