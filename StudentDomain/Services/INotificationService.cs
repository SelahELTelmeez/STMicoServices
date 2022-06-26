using StudentDomain.Models;

namespace StudentDomain.Services;
public interface INotificationService
{
    Task<bool> PushNotificationAsync(HttpClient httpClient, NotificationModel notification, CancellationToken cancellationToken);
}
