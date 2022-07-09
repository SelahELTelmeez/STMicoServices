using ChatDomain.Features.CQRS.Query;
using ChatEntities;
using ChatEntities.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatInfrastructure.Features.CQRS.Query;

public class GetConnectionIdByUserIdQueryHandler : IRequestHandler<GetConnectionIdByUserIdQuery, ICommitResult<string>>
{
    private readonly ChatDbContext _dbContext;
    public GetConnectionIdByUserIdQueryHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ICommitResult<string>> Handle(GetConnectionIdByUserIdQuery request, CancellationToken cancellationToken)
    {
        ChatSession? chatSession = await _dbContext.Set<ChatSession>().FirstOrDefaultAsync(a => a.UserId.Equals(request.UserId), cancellationToken);

        if (chatSession != null)
        {
            return ResultType.NotFound.GetValueCommitResult(string.Empty, "CATX0001", "The user is not connected");
        }
        else
        {
            return ResultType.Ok.GetValueCommitResult(chatSession!.ConnectionId);
        }
    }
}
