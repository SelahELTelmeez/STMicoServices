using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
public record GetIdentityUserByEmailAndPasswordQuery(string? Email, string PasswordHash) : IRequest<DomainEntities.IdentityUser>;