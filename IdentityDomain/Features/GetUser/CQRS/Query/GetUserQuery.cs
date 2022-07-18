using Flaminco.CommitResult;
using IdentityDomain.Features.Login.DTO.Command;

namespace IdentityDomain.Features.GetUser.CQRS.Query;

public record GetUserQuery(string? UserId) : IRequest<ICommitResult<LoginResponse>>;

