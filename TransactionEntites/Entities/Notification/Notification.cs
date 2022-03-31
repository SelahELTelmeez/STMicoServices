using System.ComponentModel.DataAnnotations.Schema;
using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Notification;

public class Notification : TrackableEntity
{
    public Guid ActorId { get; set; }
    public Guid NotifierId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsSeen { get; set; }
    public bool IsNew { get; set; }
    public int NotificationTypeId { get; set; }
    [ForeignKey(nameof(NotificationTypeId))] public NotificationType NotificationTypeFK { get; set; }
}
