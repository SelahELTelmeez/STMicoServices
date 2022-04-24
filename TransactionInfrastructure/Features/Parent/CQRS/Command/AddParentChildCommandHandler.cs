using Mapster;
using Microsoft.AspNetCore.Http;
using TransactionDomain.Features.Parent.CQRS.Command;
using TransactionDomain.Features.Parent.DTO;
using TransactionDomain.Features.Shared.DTO;
using TransactionEntites.Entities;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Parent.CQRS.Command;

public class AddParentChildCommandHandler : IRequestHandler<AddParentChildCommand, CommitResult<AddParentChildResponse>>
{
    private readonly Guid? _userId;
    private IMediator _mediator;
    private readonly TrackerDbContext _dbContext;
    private readonly IdentityClient _IdentityClient;
    private readonly NotifierClient _NotifierClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityClient _identityClient;

    public AddParentChildCommandHandler(IMediator mediator, NotifierClient NotifierClient, IdentityClient identityClient, IHttpClientFactory httpClientFactory, TrackerDbContext dbContext, IdentityClient IdentityClient, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _dbContext = dbContext;
        _NotifierClient = NotifierClient;
        _identityClient = identityClient;
        _IdentityClient = IdentityClient;
        _httpClientFactory = httpClientFactory;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult<AddParentChildResponse>> Handle(AddParentChildCommand request, CancellationToken cancellationToken)
    {
        // =========== add child  to parent================
        CommitResult<AddParentChildResponse>? subjectDetails = await _IdentityClient.AddParentChildAsync(request.AddParentChildRequest, cancellationToken);

        // =========== send invitation to child================
        if (subjectDetails.IsSuccess)
        {
            if (request.AddParentChildRequest.ChildId != null)
            {
                CommitResult<LimitedProfileResponse>? ParentLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(_userId.Value, cancellationToken);
                //InvitationType? invitationType = await _dbContext.Set<InvitationType>().SingleOrDefaultAsync(a => a.Id.Equals(1), cancellationToken);

                await _NotifierClient.SendInvitationAsync(new InvitationRequest
                {
                    InviterId = _userId.GetValueOrDefault(),
                    InvitedId = request.AddParentChildRequest.ChildId.Value,
                    Argument = null,
                    InvitationTypeId = 1,
                    IsActive = true,
                    AppenedMessage = ParentLimitedProfile.Value.FullName // + " " + invitationType.Description
                }, cancellationToken);
            }
        }
        return subjectDetails.Adapt<CommitResult<AddParentChildResponse>>();
    }
}