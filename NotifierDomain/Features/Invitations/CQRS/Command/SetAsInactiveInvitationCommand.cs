namespace NotifierDomain.Features.Invitations.CQRS.Command;

public record SetAsInactiveInvitationCommand(int InvitationId) : IRequest<CommitResult>;

