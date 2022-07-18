using ChatDomain.Features.CQRS.Command;
using ChatEntities;
using ChatEntities.Entities;

namespace ChatInfrastructure.Features.CQRS.Command;

public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand, ICommitResult>
{
    private readonly ChatDbContext _dbContext;
    public AddChatMessageCommandHandler(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ICommitResult> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Messages.Add(new Message
        {
            FromId = request.ServerMessage.FromId,
            ToId = request.ServerMessage.ToId,
            Content = request.ServerMessage.Content,
            Time = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}
