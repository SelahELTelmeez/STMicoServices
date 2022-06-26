using StudentDomain.Models;
using StudentDomain.Services;
using System.Net.Http.Json;

namespace StudentInfrastructure.Services;
public class NotificationService : INotificationService
{
    public async Task<bool> PushNotificationAsync(HttpClient httpClient, NotificationModel notification, CancellationToken cancellationToken)
    {
        var payload = new
        {
            to = notification.Token,
            content_available = true,
            notification = new
            {

                body = notification.Body,
                title = notification.Title,
                badge = 1,

            },
            data = new
            {
                priority = "high",
                key1 = notification.Type
            }
        };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/send", payload, cancellationToken: cancellationToken);
        return httpResponseMessage.IsSuccessStatusCode;
    }
}
