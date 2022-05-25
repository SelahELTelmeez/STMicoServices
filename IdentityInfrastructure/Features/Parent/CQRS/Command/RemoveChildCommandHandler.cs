using IdentityDomain.Features.Parent.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command
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
                                                                 .SingleOrDefaultAsync(a =>
                                                                 a.PrimaryId.Equals(_userId) && a.SecondaryId.Equals(request.ChildId)
                                                                 && a.RelationType.Equals(RelationType.ParentToKid), cancellationToken);



            // =========== Remove child ================
            if (IdentityRelation != null)
            {
                _dbContext.Set<IdentityRelation>().Remove(IdentityRelation);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new CommitResult
                {
                    ResultType = ResultType.Ok,
                };
            }
            // =========== Get Response ================
            return new CommitResult
            {
                ErrorCode = "X000",
                ErrorMessage = "X0000",
                ResultType = ResultType.NotFound,
            };
        }
    }
}