using NotifierDomain.Features.Invitations.CQRS.DTO.Command;

namespace NotifierDomain.Features.Invitations.CQRS.Command;
public record CreateInvitationCommand(InvitationRequest InvitationRequest) : IRequest<CommitResult>;

