using NotifierEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotifierEntities.Entities.Notifications;
public class Notification : TrackableEntity
{
    public string NotifiedId { get; set; }
    public string NotifierId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsSeen { get; set; }
    public string Argument { get; set; }
    public int NotificationTypeId { get; set; }
    [ForeignKey(nameof(NotificationTypeId))] public NotificationType NotificationTypeFK { get; set; }
}
