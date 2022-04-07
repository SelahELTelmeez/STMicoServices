using MediatR;
using ResultHandler;
using TransactionDomain.Features.Invitations.IdentityInvitation.DTO;

namespace TransactionDomain.Features.Invitations.IdentityInvitation.CQRS.Query;

public record GetIdentityInvitationsQuery() : IRequest<CommitResult<IEnumerable<IdentityInvitationResponse>>>;


