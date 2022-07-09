using IdentityDomain.Features.Parent.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using IdentityInfrastructure.HttpClients;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command;

public class AcceptChildInvitationRequestHandler : IRequestHandler<AcceptChildInvitationRequestCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _userId;
    private readonly NotifierClient _notifierClient;
    public AcceptChildInvitationRequestHandler(STIdentityDbContext dbContext,
                                  IWebHostEnvironment configuration,
                                  IHttpContextAccessor httpContextAccessor,
                                  NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _userId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
    }
    public async Task<CommitResult> Handle(AcceptChildInvitationRequestCommand request, CancellationToken cancellationToken)
    {
        IdentityRelation? identityRelation = await _dbContext.Set<IdentityRelation>().FirstOrDefaultAsync(a => a.PrimaryId == request.AddChildInvitationRequest.ParentId && a.SecondaryId == _userId && a.RelationType == RelationType.ParentToKid, cancellationToken);
        if (identityRelation != null)
        {
            return new CommitResult
            {
                ResultType = ResultType.Duplicated,
                ErrorCode = "XIDN0018",
                ErrorMessage = _resourceJsonManager["XIDN0018"]
            };
        }

        IdentityRelation IdentityRelation = new IdentityRelation
        {
            RelationType = RelationType.ParentToKid,
            PrimaryId = request.AddChildInvitationRequest.ParentId,
            SecondaryId = _userId
        };

        _dbContext.Set<IdentityRelation>().Add(IdentityRelation);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _ = _notifierClient.SetAsInActiveInvitationAsync(request.AddChildInvitationRequest.InvitationId, cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok,
        };
    }
}


