using TransactionDomain.Features.Invitations.CQRS.DTO.Command;

namespace TransactionDomain.Features.Invitations.CQRS.Command;
public record CreateInvitationCommand(InvitationRequest InvitationRequest) : IRequest<CommitResult>;

