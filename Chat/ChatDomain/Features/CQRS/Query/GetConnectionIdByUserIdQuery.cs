namespace ChatDomain.Features.CQRS.Query;

public record GetConnectionIdByUserIdQuery(string UserId) : IRequest<ICommitResult<string>>;
