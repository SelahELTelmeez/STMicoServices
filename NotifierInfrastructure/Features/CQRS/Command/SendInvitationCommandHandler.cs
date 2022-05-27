using Flaminco.CommitResult;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierDomain.Models;
using NotifierDomain.Services;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command;
public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, ICommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IdentityClient _identityClient;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    public SendInvitationCommandHandler(NotifierDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor,
                                          IdentityClient identityClient,
                                          INotificationService notification,
                                          IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _identityClient = identityClient;
        _notification = notification;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<ICommitResult> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        Invitation? invitation = await _dbContext.Set<Invitation>()
                                                 .Where(a => a.InviterId.Equals(request.InvitationRequest.InviterId)
                                                         && a.InvitedId.Equals(request.InvitationRequest.InvitedId)
                                                         && a.Argument.Equals(request.InvitationRequest.Argument)
                                                         && a.Status == NotifierEntities.Entities.Shared.InvitationStatus.None
                                                         && a.IsActive == true)
                                                 .SingleOrDefaultAsync(cancellationToken);

        if (invitation != null)
        {
            return Flaminco.CommitResult.ResultType.Duplicated.GetCommitResult("X0000", _resourceJsonManager["X0000"]);
        }

        InvitationType? invitationType = await _dbContext.Set<InvitationType>().SingleOrDefaultAsync(a => a.Id.Equals(request.InvitationRequest.InvitationTypeId), cancellationToken);
        if (invitationType == null)
        {
            return Flaminco.CommitResult.ResultType.NotFound.GetCommitResult("X0000", _resourceJsonManager["X0000"]);
        }

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetLimitedProfilesAsync(new Guid[] { request.InvitationRequest.InvitedId, request.InvitationRequest.InviterId }, cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return Flaminco.CommitResult.ResultType.Invalid.GetCommitResult(limitedProfiles.ErrorCode, limitedProfiles.ErrorMessage);
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

        return Flaminco.CommitResult.ResultType.Ok.GetCommitResult();

    }
}