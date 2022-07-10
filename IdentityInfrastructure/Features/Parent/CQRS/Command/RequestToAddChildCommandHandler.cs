using Flaminco.CommitResult;
using IdentityDomain.Features.Parent.CQRS.Command;
using IdentityInfrastructure.HttpClients;
using Microsoft.AspNetCore.Http;
using SharedModule.DTO;

namespace IdentityDomain.Features.RequestToAddChild.CQRS.Command;

public class RequestToAddChildCommandHandler : IRequestHandler<RequestToAddChildCommand, ICommitResult>
{
    private readonly NotifierClient _notifierClient;
    private readonly Guid? _parentId;
    public RequestToAddChildCommandHandler(NotifierClient notifierClient,
                                           IHttpContextAccessor httpContextAccessor)
    {
        _notifierClient = notifierClient;
        _parentId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<ICommitResult?> Handle(RequestToAddChildCommand request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResult(await _notifierClient.SendInvitationAsync(new InvitationRequest
        {
            InviterId = _parentId.GetValueOrDefault(),
            InvitedId = request.ChildId,
            Argument = _parentId.GetValueOrDefault().ToString(),
            InvitationTypeId = 1,
            IsActive = true,
            AppenedMessage = string.Empty
        }, cancellationToken));

    }
}


