using IdentityDomain.Features.Login.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.GetUser.CQRS.Query;

public record GetUserQuery() : IRequest<CommitResult<LoginResponse>>;

