using DomainEntities = IdentityEntities.Entities.Identities;

namespace IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
public record GetIdentityUserByMobileAndPasswordQuery(string? MobileNumber, string PasswordHash) : IRequest<DomainEntities.IdentityUser>;