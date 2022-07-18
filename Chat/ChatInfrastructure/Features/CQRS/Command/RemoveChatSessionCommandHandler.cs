using ChatDomain.Features.CQRS.Command;
using ChatEntities;
using ChatEntities.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatInfrastructure.Features.CQRS.Command;

public class RemoveChatSessionCommandHandler : IRequestHandler<RemoveChatSessionCommand, ICommitResult>
{
    private readonly ChatDbContext _dbContext;
    public RemoveChatSessionCommandHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ICommitResult> Handle(RemoveChatSessionCommand request, CancellationToken cancellationToken)
    {
        ChatSession? chatSession = await _dbContext.Set<ChatSession>().FirstOrDefaultAsync(a => a.ConnectionId.Equals(request.ConnectionId), cancellationToken);

        if (chatSession != null)
        {
            _dbContext.Set<ChatSession>().Remove(chatSession);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return ResultType.Ok.GetCommitResult();
    }
}
