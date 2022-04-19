
using Microsoft.EntityFrameworkCore;
using NotifierEntities.Entities.Invitations;
using NotifierEntities.Entities.Notifications;

namespace NotifierEntities.Entities;
public class NotifierDbContext : DbContext
{
    public NotifierDbContext(DbContextOptions<NotifierDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<InvitationType> InvitationTypes { get; set; }
}
