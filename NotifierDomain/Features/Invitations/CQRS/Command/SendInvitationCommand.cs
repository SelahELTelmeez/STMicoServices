using NotifierDomain.Features.Invitations.CQRS.DTO.Command;

namespace NotifierDomain.Features.Invitations.CQRS.Command;
public record SendInvitationCommand(InvitationRequest InvitationRequest) : IRequest<CommitResult>;

