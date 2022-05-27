using Flaminco.CommitResult;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierDomain.Models;
using NotifierDomain.Services;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command;
public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, ICommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IdentityClient _identityClient;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;

    public SendNotificationCommandHandler(NotifierDbContext dbContext,
                                            IdentityClient identityClient,
                                            IWebHostEnvironment configuration,
                                            IHttpContextAccessor httpContextAccessor,
                                            INotificationService notification,
                                            IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _identityClient = identityClient;
        _notification = notification;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<ICommitResult> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        Notification? invitation = await _dbContext.Set<Notification>()
           .Where(a => a.NotifierId.Equals(request.NotificationRequest.NotifierId)
                    && a.NotifiedId.Equals(request.NotificationRequest.NotifiedId)
                    && a.Argument.Equals(request.NotificationRequest.Argument))
           .SingleOrDefaultAsync(cancellationToken);


        if (invitation != null)
        {
            return Flaminco.CommitResult.ResultType.Duplicated.GetCommitResult("X0000", _resourceJsonManager["X0000"]);

        }

        NotificationType? notificationType = await _dbContext.Set<NotificationType>().SingleOrDefaultAsync(a => a.Id.Equals(request.NotificationRequest.NotificationTypeId), cancellationToken);

        if (notificationType == null)
        {
            return Flaminco.CommitResult.ResultType.NotFound.GetCommitResult("X0000", _resourceJsonManager["X0000"]);
        }

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetLimitedProfilesAsync(new Guid[] { request.NotificationRequest.NotifierId, request.NotificationRequest.NotifiedId }, cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return Flaminco.CommitResult.ResultType.Invalid.GetCommitResult(limitedProfiles.ErrorCode, limitedProfiles.ErrorMessage);
        }

        LimitedProfileResponse notifierProfile = limitedProfiles.Value.SingleOrDefault(a => a.UserId.Equals(request.NotificationRequest.NotifierId));
        LimitedProfileResponse notifiedProfile = limitedProfiles.Value.SingleOrDefault(a => a.UserId.Equals(request.NotificationRequest.NotifiedId));

        string notificationBody = $"{notifierProfile.FullName} {notificationType.Description} {request.NotificationRequest.AppenedMessage}";

        _dbContext.Set<Notification>().Add(new Notification
        {
            Argument = request.NotificationRequest.Argument,
            NotifiedId = request.NotificationRequest.NotifiedId,
            NotifierId = request.NotificationRequest.NotifierId,
            NotificationTypeId = request.NotificationRequest.NotificationTypeId,
            Message = notificationBody,
            Title = notificationType.Name,
            IsSeen = false
        });

        await _dbContext.SaveChangesAsync(cancellationToken);


        await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
        {
            Token = notifiedProfile.NotificationToken,
            Type = request.NotificationRequest.NotificationTypeId,
            Title = notificationType.Name,
            Body = notificationBody

        }, cancellationToken);

        return Flaminco.CommitResult.ResultType.Ok.GetCommitResult();
    }
}