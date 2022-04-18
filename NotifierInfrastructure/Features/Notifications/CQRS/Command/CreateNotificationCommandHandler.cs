using Mapster;
using NotifierDomain.Features.Notification.CQRS.Command;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;

namespace NotifierInfrastructure.Features.Notifications.CQRS.Command;
public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, CommitResult>
{
    private readonly NotifierDbContext _dbContext;
    public CreateNotificationCommandHandler(NotifierDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Set<Notification>().Add(request.NotificationRequest.Adapt<Notification>());
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResultProvider().SuccessResult();
    }
}