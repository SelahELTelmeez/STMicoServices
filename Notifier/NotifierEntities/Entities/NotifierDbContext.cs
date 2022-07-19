
using Microsoft.EntityFrameworkCore;
using NotifierEntities.Entities.Invitations;
using NotifierEntities.Entities.Notifications;

namespace NotifierEntities.Entities;
public class NotifierDbContext : DbContext
{
    public NotifierDbContext(DbContextOptions<NotifierDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<InvitationType> InvitationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invitation>()
          .Property(a => a.InvitedId)
          .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<Invitation>()
          .Property(a => a.InviterId)
          .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<Notification>()
          .Property(a => a.NotifiedId)
          .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<Notification>()
         .Property(a => a.NotifierId)
         .HasConversion(v => v.ToLower(), v => v.ToLower());
    }
}