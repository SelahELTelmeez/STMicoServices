using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Invitations.CQRS.Command;
using NotifierDomain.Features.Shared.DTO;
using NotifierDomain.Models;
using NotifierDomain.Services;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.Invitations.CQRS.Command;
public class CreateInvitationCommandHandler : IRequestHandler<CreateInvitationCommand, CommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IdentityClient _identityClient;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    public CreateInvitationCommandHandler(NotifierDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor,
                                          IdentityClient identityClient,
                                          INotificationService notification)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _identityClient = identityClient;
        _notification = notification;
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

        InvitationType? invitationType = await _dbContext.Set<InvitationType>().SingleOrDefaultAsync(a => a.Id.Equals(request.InvitationRequest.InvitationTypeId), cancellationToken);
        if (invitationType == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetIdentityLimitedProfilesAsync(new Guid[] { request.InvitationRequest.InvitedId, request.InvitationRequest.InviterId }, cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return limitedProfiles.Adapt<CommitResult>();
        }

        LimitedProfileResponse inviterProfile = limitedProfiles.Value.SingleOrDefault(a => a.UserId.Equals(request.InvitationRequest.InvitedId));
        LimitedProfileResponse invitedProfile = limitedProfiles.Value.SingleOrDefault(a => a.UserId.Equals(request.InvitationRequest.InviterId));

        string notificationBody = $"{inviterProfile.FullName} {invitationType.Description} {request.InvitationRequest.AppenedMessage}";

        _dbContext.Set<Invitation>().Add(new Invitation
        {
            Argument = request.InvitationRequest.Argument,
            InvitedId = request.InvitationRequest.InvitedId,
            InviterId = request.InvitationRequest.InviterId,
            IsActive = true,
            InvitationTypeId = request.InvitationRequest.InvitationTypeId,
            Status = NotifierEntities.Entities.Shared.InvitationStatus.None,
            Message = notificationBody,
            Title = invitationType.Name,
            IsSeen = false
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
        {
            Token = invitedProfile.NotificationToken,
            Type = request.InvitationRequest.InvitationTypeId,
            Title = invitationType.Name,
            Body = notificationBody
        }, cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}