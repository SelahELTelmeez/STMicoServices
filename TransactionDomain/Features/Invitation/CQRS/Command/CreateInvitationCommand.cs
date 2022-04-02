using MediatR;
using ResultHandler;
using TransactionDomain.Features.Invitation.DTO;

namespace TransactionDomain.Features.Invitation.CQRS.Command;

public record CreateInvitationCommand(InvitationRequest InvitationRequest) : IRequest<CommitResult>;

