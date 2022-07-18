using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command
{
    public class SetAsSeenInvitationCommandHandler : IRequestHandler<SetAsSeenInvitationCommand, ICommitResult>
    {
        private readonly NotifierDbContext _dbContext;
        private readonly string? _userId;

        public SetAsSeenInvitationCommandHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<ICommitResult> Handle(SetAsSeenInvitationCommand request, CancellationToken cancellationToken)
        {
            IEnumerable<Invitation> invitations = await _dbContext.Set<Invitation>().Where(a => a.IsSeen == false && a.IsActive == true && a.InvitedId.Equals(_userId)).ToListAsync(cancellationToken);

            foreach (var invitation in invitations)
            {
                invitation.IsSeen = true;
            }

            _dbContext.UpdateRange(invitations);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return ResultType.Ok.GetCommitResult();
        }
    }
}
