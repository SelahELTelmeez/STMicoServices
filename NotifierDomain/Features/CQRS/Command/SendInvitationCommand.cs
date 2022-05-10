using NotifierDomain.Features.CQRS.DTO.Command;

namespace NotifierDomain.Features.CQRS.Command;
public record SendInvitationCommand(InvitationRequest InvitationRequest) : IRequest<CommitResult>;

