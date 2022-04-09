namespace TransactionDomain.Features.Notification.DTO.Command;
public class NotificationRequest
{
    public Guid ActorId { get; set; }
    public Guid NotifierId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsSeen { get; set; }
    public bool IsNew { get; set; }
    public int NotificationTypeId { get; set; }
}