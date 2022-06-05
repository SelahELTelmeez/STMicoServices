using Flaminco.CommitResult;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;

namespace NotifierInfrastructure.Features.CQRS.Command
{
    public class SetAsSeenInvitationCommandHandler : IRequestHandler<SetAsSeenInvitationCommand, ICommitResult>
    {
        private readonly NotifierDbContext _dbContext;
        public SetAsSeenInvitationCommandHandler(NotifierDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICommitResult> Handle(SetAsSeenInvitationCommand request, CancellationToken cancellationToken)
        {
            IEnumerable<Invitation> invitations = await _dbContext.Set<Invitation>().Where(a => a.IsSeen == false).ToListAsync(cancellationToken);

            foreach (var invitation in invitations)
            {
                invitation.IsSeen = true;
            }

            _dbContext.UpdateRange(invitations);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Flaminco.CommitResult.ResultType.Ok.GetCommitResult();
        }
    }
}
