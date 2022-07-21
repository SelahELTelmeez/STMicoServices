using Microsoft.Extensions.Configuration;
using NotifierDomain.Models;
using NotifierDomain.Services;
using System.Net.Http.Json;

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
        var payload = new
        {
            to = model.Token,
            content_available = true,
            notification = new
            {
                body = model.Body,
                title = model.Title,
                badge = 1,
            },
            data = new
            {
                priority = "high",
                key1 = model.Type,
                key2 = "value2"
            }
        };

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("https://fcm.googleapis.com/fcm/send", payload, cancellationToken);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
