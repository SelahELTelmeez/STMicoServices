using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
public record GetIdentityUserByIdQuery(Guid? IdentityUserId) : IRequest<DomainEntities.IdentityUser>;
