using TeacherDomain.Models;

namespace TeacherDomain.Services;
public interface INotificationService
{
    Task<bool> PushNotificationAsync(HttpClient httpClient, NotificationModel notification, CancellationToken cancellationToken);
}
