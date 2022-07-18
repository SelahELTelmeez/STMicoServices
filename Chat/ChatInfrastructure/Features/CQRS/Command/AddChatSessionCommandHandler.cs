using ChatDomain.Features.CQRS.Command;
using ChatEntities;
using ChatEntities.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.Extensions;

namespace ChatInfrastructure.Features.CQRS.Command;

public class AddChatSessionCommandHandler : IRequestHandler<AddChatSessionCommand, ICommitResult>
{
    private readonly ChatDbContext _dbContext;
    private readonly string? _userId;
    public AddChatSessionCommandHandler(ChatDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<ICommitResult> Handle(AddChatSessionCommand request, CancellationToken cancellationToken)
    {
        ChatSession? userSession = await _dbContext.Set<ChatSession>().FirstOrDefaultAsync(a => a.UserId.Equals(_userId), cancellationToken);

        if (userSession == null)
        {
            _dbContext.Set<ChatSession>().Add(new ChatSession
            {
                ConnectionId = request.ConnectionId,
                UserId = _userId
            });
        }
        else // current user is alredy existed in the session table., update the connection only.
        {
            userSession.ConnectionId = request.ConnectionId;
            _dbContext.Set<ChatSession>().Update(userSession);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}
