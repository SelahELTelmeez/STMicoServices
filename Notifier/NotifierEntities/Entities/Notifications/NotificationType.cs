using NotifierEntities.Entities.Shared;

namespace NotifierEntities.Entities.Notifications;
public class NotificationType : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }

}
