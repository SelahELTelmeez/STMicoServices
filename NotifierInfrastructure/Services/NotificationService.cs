using CorePush.Google;
using Microsoft.Extensions.Configuration;
using NotifierDomain.Models;
using NotifierDomain.Services;

namespace NotifierInfrastructure.Services;
public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    public NotificationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<bool> PushNotificationAsync(HttpClient httpClient, NotificationModel model, CancellationToken cancellationToken)
    {
        DataPayload dataPayload = new DataPayload
        {
            Title = model.Title,
            Body = model.Body
        };

        GoogleNotification notification = new GoogleNotification
        {
            Data = dataPayload,
            Notification = dataPayload
        };

        var fcm = new FcmSender(new FcmSettings
        {
            SenderId = _configuration["FCM:SenderId"],
            ServerKey = _configuration["FCM:ServerKey"],
        }, httpClient);

        var fcmSendResponse = await fcm.SendAsync(model.Token, notification, cancellationToken);

        if (fcmSendResponse.IsSuccess())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
