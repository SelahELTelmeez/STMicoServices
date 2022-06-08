using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
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
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetClassesCurrentStatusQueryHandler(NotifierDbContext dbContext,
                                                   IWebHostEnvironment configuration,
                                                   IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }

        public async Task<ICommitResults<ClassStatusResponse>> Handle(GetClassesCurrentStatusQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<Invitation> invitations = await _dbContext.Set<Invitation>().Where(a => a.InvitedId == _userId).ToListAsync(cancellationToken);

            if (!invitations.Any())
            {
                return ResultType.Empty.GetValueCommitResults(Array.Empty<ClassStatusResponse>(), "X0007", _resourceJsonManager["X0007"]);
            }

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

            return ResultType.Ok.GetValueCommitResults(GetStatus());
        }
    }
}
