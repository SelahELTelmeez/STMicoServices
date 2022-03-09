using IdentityDomain.Features.Register.DTO.Queries;

namespace IdentityDomain.Features.Register.CQRS.Queries;
public record IdentityCheckUserExistenceQuery(IdentityCheckUserExistenceRequestDTO IdentityLoginCheckUserExistenceRequest) : IRequest<bool>;

