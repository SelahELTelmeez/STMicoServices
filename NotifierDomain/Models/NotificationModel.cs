using System.Text.Json.Serialization;

namespace NotifierDomain.Models;
public class NotificationModel
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Token { get; set; }
    public int Type { get; set; }
}

public class DataPayload
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("body")]
    public string Body { get; set; }
}
public class GoogleNotification
{
    [JsonPropertyName("priority")]
    public string Priority { get; set; } = "high";
    [JsonPropertyName("data")]
    public DataPayload Data { get; set; }
    [JsonPropertyName("notification")]
    public DataPayload Notification { get; set; }
}

public class FcmNotificationSetting
{
    public string SenderId { get; set; }
    public string ServerKey { get; set; }
}