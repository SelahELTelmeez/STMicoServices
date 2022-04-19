using NotifierDomain.Features.Invitations.CQRS.DTO.Query;

namespace NotifierDomain.Features.Invitations.CQRS.Query;
public record GetIdentityInvitationsQuery() : IRequest<CommitResults<IdentityInvitationResponse>>;


