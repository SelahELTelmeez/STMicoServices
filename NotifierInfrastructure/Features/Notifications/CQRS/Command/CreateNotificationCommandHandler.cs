using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Notification.CQRS.Command;
using NotifierDomain.Features.Shared.DTO;
using NotifierDomain.Models;
using NotifierDomain.Services;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Notifications;
using NotifierInfrastructure.HttpClients;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.Notifications.CQRS.Command;
public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, CommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IdentityClient _identityClient;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;

    public CreateNotificationCommandHandler(NotifierDbContext dbContext,
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
    public async Task<CommitResult> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        Notification? invitation = await _dbContext.Set<Notification>()
           .Where(a => a.NotifierId.Equals(request.NotificationRequest.NotifierId)
                    && a.NotifiedId.Equals(request.NotificationRequest.NotifiedId)
                    && a.Argument.Equals(request.NotificationRequest.Argument))
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

        NotificationType? notificationType = await _dbContext.Set<NotificationType>().SingleOrDefaultAsync(a => a.Id.Equals(request.NotificationRequest.NotificationTypeId), cancellationToken);

        if (notificationType == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetIdentityLimitedProfilesAsync(new Guid[] { request.NotificationRequest.NotifierId, request.NotificationRequest.NotifiedId }, cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return limitedProfiles.Adapt<CommitResult>();
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

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}