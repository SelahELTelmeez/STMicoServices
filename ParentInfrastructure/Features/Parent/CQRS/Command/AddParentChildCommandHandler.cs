using MediatR;
using Microsoft.AspNetCore.Http;
using ParentDomain.Features.Parent.CQRS.Command;
using ParentDomain.Features.Parent.DTO;
using ParentDomain.Features.Shared.DTO;
using ParentInfrastructure.HttpClients;
using ResultHandler;
using SharedModule.Extensions;

namespace ParentInfrastructure.Features.Parent.CQRS.Command;

public class AddParentChildCommandHandler : IRequestHandler<AddParentChildCommand, CommitResult<AddParentChildResponse>>
{
    private readonly Guid? _userId;
    private readonly IdentityClient _IdentityClient;
    private readonly NotifierClient _NotifierClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public AddParentChildCommandHandler(NotifierClient NotifierClient,
                                         IdentityClient IdentityClient,
                                         IHttpClientFactory httpClientFactory,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _NotifierClient = NotifierClient;
        _IdentityClient = IdentityClient;
        _httpClientFactory = httpClientFactory;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult<AddParentChildResponse>> Handle(AddParentChildCommand request, CancellationToken cancellationToken)
    {
        // =========== add child  to parent================
        CommitResult<AddParentChildResponse>? ParentChild = await _IdentityClient.AddParentChildAsync(request.AddParentChildRequest, cancellationToken);

        // =========== send invitation to child================
        if (ParentChild.IsSuccess)
        {
            if (request.AddParentChildRequest.ChildId != null)
            {
                CommitResult<LimitedProfileResponse>? ParentLimitedProfile = await _IdentityClient.GetIdentityLimitedProfileAsync(_userId.Value, cancellationToken);
                //InvitationType? invitationType = await _dbContext.Set<InvitationType>().SingleOrDefaultAsync(a => a.Id.Equals(1), cancellationToken);

                //Bug
                //await _NotifierClient.SendInvitationAsync(new InvitationRequest
                //{
                //    InviterId = _userId.GetValueOrDefault(),
                //    InvitedId = request.AddParentChildRequest.ChildId.Value,
                //    Argument = null,
                //    InvitationTypeId = 1,
                //    IsActive = true,
                //    AppenedMessage = ParentLimitedProfile?.Value?.FullName// + " " + invitationType.Description
                //}, cancellationToken);
            }
        }
        return ParentChild;
    }
}