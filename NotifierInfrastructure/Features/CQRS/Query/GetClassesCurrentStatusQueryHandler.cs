using Flaminco.CommitResult;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Query
{
    public class GetClassesCurrentStatusQueryHandler : IRequestHandler<GetClassesCurrentStatusQuery, ICommitResults<ClassStatusResponse>>
    {
        private readonly NotifierDbContext _dbContext;
        private readonly Guid? _userId;
        public GetClassesCurrentStatusQueryHandler(NotifierDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<ICommitResults<ClassStatusResponse>> Handle(GetClassesCurrentStatusQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<Invitation> invitations = await _dbContext.Set<Invitation>().Where(a => a.InvitedId == _userId).ToListAsync(cancellationToken);

            IEnumerable<ClassStatusResponse> GetStatus()
            {
                foreach (var invitation in invitations)
                {
                    if (int.TryParse(invitation.Argument, out int classId))
                    {
                        if (request.ClassIds.Contains(classId))
                        {
                            yield return new ClassStatusResponse
                            {
                                ClassId = classId,
                                Status = (int)invitation.Status
                            };
                        }
                    }
                }
            }

            return Flaminco.CommitResult.ResultType.Ok.GetValueCommitResults(GetStatus());
        }
    }
}
