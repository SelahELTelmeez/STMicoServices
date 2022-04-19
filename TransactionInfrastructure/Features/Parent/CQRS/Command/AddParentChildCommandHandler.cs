using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Invitations.CQRS.Command;
using TransactionDomain.Features.Invitations.CQRS.DTO.Command;
using TransactionDomain.Features.Parent.CQRS.Command;
using TransactionDomain.Features.Parent.DTO;
using TransactionDomain.Features.Shared.DTO;
using TransactionDomain.Models;
using TransactionDomain.Services;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Invitation;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Parent.CQRS.Command;

public class AddParentChildCommandHandler : IRequestHandler<AddParentChildCommand, CommitResult<AddParentChildResponse>>
{
    private readonly Guid? _userId;
    private IMediator _mediator;
    private readonly TrackerDbContext _dbContext;
    private readonly IdentityClient _IdentityClient;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityClient _identityClient;

    public AddParentChildCommandHandler(IMediator mediator, IdentityClient identityClient, IHttpClientFactory httpClientFactory, INotificationService notification, TrackerDbContext dbContext, IdentityClient IdentityClient, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _dbContext = dbContext;
        _notification = notification;
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
                InvitationType? invitationType = await _dbContext.Set<InvitationType>().SingleOrDefaultAsync(a => a.Id.Equals(1), cancellationToken);

                CommitResult createInvitationResult = await _mediator.Send(new CreateInvitationCommand(new InvitationRequest
                {
                    InviterId = _userId.GetValueOrDefault(),
                    InvitedId = (Guid)request.AddParentChildRequest.ChildId,
                    // Argument = request.ClassId.ToString(),
                    InvitationTypeId = 1,
                    IsActive = true,
                }), cancellationToken);

                if (!createInvitationResult.IsSuccess)
                {
                    return new CommitResult<AddParentChildResponse>
                    {
                        ResultType = createInvitationResult.ResultType,
                        Value = subjectDetails.Value
                    };
                }
                CommitResult<LimitedProfileResponse>? ParentLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(_userId.Value, cancellationToken);
                await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
                {
                    // Token = TeacherLimitedProfile.Value.NotificationToken,
                    Type = 1,
                    Title = invitationType.Name,
                    Body = ParentLimitedProfile.Value.FullName + " " + invitationType.Description

                }, cancellationToken);
            }
        }
        return subjectDetails.Adapt<CommitResult<AddParentChildResponse>>();
    }
}