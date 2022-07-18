namespace NotifierDomain.Features.DTO.Query;
public class NotificationResponse
{
    public string NotifiedId { get; set; }
    public string NotifierId { get; set; }
    public int NotificationId { get; set; }
    public string Description { get; set; }
    public bool IsSeen { get; set; }
    public string AvatarUrl { get; set; }
    public string Argument { get; set; }
    public DateTime CreatedOn { get; set; }
    public int Type { get; set; }
}
