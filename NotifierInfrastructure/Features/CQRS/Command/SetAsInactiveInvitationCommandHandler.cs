using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command;

public class SetAsInactiveInvitationCommandHandler : IRequestHandler<SetAsInactiveInvitationCommand, ICommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    public SetAsInactiveInvitationCommandHandler(NotifierDbContext dbContext,
                                                 IWebHostEnvironment configuration,
                                                 IHttpContextAccessor httpContextAccessor)

    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult> Handle(SetAsInactiveInvitationCommand request, CancellationToken cancellationToken)
    {
        Invitation? invitation = await _dbContext.Set<Invitation>().SingleOrDefaultAsync(a => a.Id.Equals(request.InvitationId), cancellationToken);

        if (invitation == null)
        {
            return ResultType.NotFound.GetCommitResult("X0005", _resourceJsonManager["X0005"]);
        }

        invitation.IsActive = false;
        invitation.Status = NotifierEntities.Entities.Shared.InvitationStatus.None;
        _dbContext.Set<Invitation>().Update(invitation);
        return ResultType.Ok.GetCommitResult();
    }
}
