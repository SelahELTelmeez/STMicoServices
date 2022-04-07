using MediatR;
using ResultHandler;
using TransactionDomain.Features.Invitations.CreateInvitation.DTO;

namespace TransactionDomain.Features.Invitations.CreateInvitation.CQRS.Command;

public record CreateInvitationCommand(InvitationRequest InvitationRequest) : IRequest<CommitResult>;

