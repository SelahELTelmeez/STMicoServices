
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
        modelBuilder.Entity<InvitationType>().HasData(
        new InvitationType { Id = 1, Name = "دعوة من ولي أمر	يدعوك لقبول طلب إضافتك إلي قائمة طلابه" },
        new InvitationType { Id = 2, Name = "دعوة صداقة	يدعوك لقبول طلب الصداقة" },
        new InvitationType { Id = 3, Name = "دعوة من معلم	يدعوك لقبول طلب إضافتك إلي قائمة طلابه" },
        new InvitationType { Id = 4, Name = "طلب اشتراك	يدعوك لقبول اشتراكه في فصل -" },
        new InvitationType { Id = 5, Name = "طلب إعادة إشتراك	يدعوك لقبول إعادة اشتراكه في فصل -" });
        base.OnModelCreating(modelBuilder);


    }
}