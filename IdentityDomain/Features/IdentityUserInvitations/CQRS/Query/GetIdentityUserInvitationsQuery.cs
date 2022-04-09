using IdentityDomain.Features.IdentityUserInvitations.DTO.Query;
using ResultHandler;

namespace IdentityDomain.Features.IdentityUserInvitations.CQRS.Query;
public record GetIdentityUserInvitationsQuery(List<Guid> InviterIds) : IRequest<CommitResults<IdentityUserInvitationResponse>>;