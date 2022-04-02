﻿using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Notification;

public class NotificationType : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; }

}
