using IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityUserTransaction.CQRS.Command
{
    public class RemoveChildCommandHandler : IRequestHandler<RemoveChildCommand, CommitResult>
    {
        private readonly STIdentityDbContext _dbContext;
        private readonly Guid? _userId;

        public RemoveChildCommandHandler(STIdentityDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<CommitResult> Handle(RemoveChildCommand request, CancellationToken cancellationToken)
        {
            // =========== Check for the relation of this student and parent existance first ================
            IdentityRelation? IdentityRelation = await _dbContext.Set<IdentityRelation>()
                                                   .FirstOrDefaultAsync(a => a.PrimaryId.Equals(_userId)
                                                                        && a.SecondaryId.Equals(request.RemoveChildRequest.ChildId)
                                                                        && a.RelationType.Equals(1), cancellationToken);

            // =========== Remove child ================
            if (IdentityRelation != null)
            {
                _dbContext.Set<IdentityRelation>().Remove(IdentityRelation);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            // =========== Get Response ================
            return new CommitResult<int>
            {
                ResultType = ResultType.Ok,
            };
        }
    }
}