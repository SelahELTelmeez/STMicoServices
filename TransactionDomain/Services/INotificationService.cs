using TransactionDomain.Models;

namespace TransactionDomain.Services;
public interface INotificationService
{
    Task<bool> PushNotificationAsync(HttpClient httpClient, NotificationModel notification, CancellationToken cancellationToken);
}
