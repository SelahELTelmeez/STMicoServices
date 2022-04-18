using JsonLocalizer;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Invitations.CQRS.Command;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.Utilities;
using ResultHandler;
using NotifierEntities.Entities;

namespace NotifierInfrastructure.Features.Invitations.CQRS.Command;
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateInvitationCommandHandler(NotifierDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult> Handle(CreateInvitationCommand request, CancellationToken cancellationToken)
    {
        Invitation? invitation = await _dbContext.Set<Invitation>()
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
        _dbContext.Set<Invitation>().Add(request.InvitationRequest.Adapt<Invitation>());
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResultProvider().SuccessResult();
    }
}