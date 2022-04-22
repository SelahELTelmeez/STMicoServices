namespace NotifierDomain.Features.Notification.DTO.Query;
public class NotificationResponse
{
    public int NotificationId { get; set; }
    public string Description { get; set; }
    public bool IsSeen { get; set; }
    public string AvatarUrl { get; set; }
    public string Argument { get; set; }
    public DateTime CreatedOn { get; set; }
}
