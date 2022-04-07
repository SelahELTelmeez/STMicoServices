using IdentityDomain.Features.IdentityUserInvitations.DTO.Query;

namespace IdentityDomain.Features.IdentityUserInvitations.CQRS.Query;
public record GetIdentityUserInvitationsQuery(List<Guid> InviterIds) : IRequest<List<IdentityUserInvitationResponse>>;