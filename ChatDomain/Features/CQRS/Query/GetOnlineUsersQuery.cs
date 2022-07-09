using ChatDomain.Features.DTO;

namespace ChatDomain.Features.CQRS.Query;

public record GetOnlineUsersQuery() : IRequest<ICommitResults<OnlineUser>>;
