using NotifierDomain.Features.Invitations.CQRS.DTO.Query;

namespace NotifierDomain.Features.Invitations.CQRS.Query;
public record GetInvitationsQuery() : IRequest<CommitResults<InvitationResponse>>;


