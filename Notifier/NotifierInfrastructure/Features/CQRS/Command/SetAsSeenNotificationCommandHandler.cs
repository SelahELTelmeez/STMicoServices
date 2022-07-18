using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command
{
    public class SetAsSeenNotificationCommandHandler : IRequestHandler<SetAsSeenNotificationCommand, ICommitResult>
    {
        private readonly NotifierDbContext _dbContext;
        private readonly string? _userId;

        public SetAsSeenNotificationCommandHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<ICommitResult> Handle(SetAsSeenNotificationCommand request, CancellationToken cancellationToken)
        {
            IEnumerable<Notification> notifications = await _dbContext.Set<Notification>().Where(a => a.IsSeen == false && a.NotifiedId.Equals(_userId)).ToListAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                notification.IsSeen = true;
            }

            _dbContext.UpdateRange(notifications);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}
