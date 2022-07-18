using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
public record GetIdentityUserByIdQuery(string? IdentityUserId) : IRequest<DomainEntities.IdentityUser>;
