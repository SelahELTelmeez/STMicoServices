using TransactionDomain.Features.Invitations.CQRS.DTO.Query;

namespace TransactionDomain.Features.Invitations.CQRS.Query;
public record GetIdentityInvitationsQuery() : IRequest<CommitResults<IdentityInvitationResponse>>;


