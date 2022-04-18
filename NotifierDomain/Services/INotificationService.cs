using NotifierDomain.Models;

namespace NotifierDomain.Services;
public interface INotificationService
{
    Task<bool> PushNotificationAsync(HttpClient httpClient, NotificationModel notification, CancellationToken cancellationToken);
}
