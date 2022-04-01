using MediatR;
using ResultHandler;
using TransactionDomain.Features.Notification.CQRS.Command;
using TransactionDomain.Features.Notification.DTO;
using TransactionEntites.Entities;
using DomainEntities = TransactionEntites.Entities.Notification;
using Mapster;

namespace TransactionInfrastructure.Features.Notification.CQRS.Command;
public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, CommitResult<NotificationResponse>>
{
    private readonly StudentTrackerDbContext _dbContext;
    public CreateNotificationCommandHandler(StudentTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult<NotificationResponse>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        _dbContext.Set<DomainEntities.Notification>().Add(request.NotificationRequest.Adapt<DomainEntities.Notification>());
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult<NotificationResponse>
        {
            ResultType = ResultType.Ok
        };
    }
}