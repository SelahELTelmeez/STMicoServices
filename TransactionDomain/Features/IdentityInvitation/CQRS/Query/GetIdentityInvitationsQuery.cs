using MediatR;
using ResultHandler;
using TransactionDomain.Features.IdentityInvitation.DTO;

namespace TransactionDomain.Features.IdentityInvitation.CQRS.Query;

public record GetIdentityInvitationsQuery(Guid IdentityId) : IRequest<CommitResult<IEnumerable<IdentityInvitationResponse>>>;


