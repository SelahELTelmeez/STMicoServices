using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Invitations.CQRS.Command;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Invitation;

namespace TransactionInfrastructure.Features.Invitations.CQRS.Command;
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateInvitationCommandHandler(TrackerDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.Invitation? invitation = await _dbContext.Set<DomainEntities.Invitation>()
            .Where(a => a.InviterId.Equals(request.InvitationRequest.InviterId) && a.InvitedId.Equals(request.InvitationRequest.InvitedId) && a.Argument.Equals(request.InvitationRequest.Argument))
            .SingleOrDefaultAsync(cancellationToken);


        if (invitation != null)
        {
            return new CommitResult
            {
                ResultType = ResultType.Duplicated,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }
        _dbContext.Set<DomainEntities.Invitation>().Add(request.InvitationRequest.Adapt<DomainEntities.Invitation>());
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}