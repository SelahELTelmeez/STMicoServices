namespace ChatDomain.Features.CQRS.Query;

public record GetConnectionIdByUserIdQuery(Guid UserId) : IRequest<ICommitResult<string>>;
