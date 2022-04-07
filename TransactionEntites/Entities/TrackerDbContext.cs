using Microsoft.EntityFrameworkCore;
using TransactionEntites.Entities.Rewards;
using TransactionEntites.Entities.Trackers;
using DomainEntities = TransactionEntites.Entities.Notification;
using DomainEntitiesInvitation = TransactionEntites.Entities.Invitation;

namespace TransactionEntites.Entities;
public class TrackerDbContext : DbContext
{
    public TrackerDbContext(DbContextOptions<TrackerDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<StudentActivityTracker> StudentActivityTracker { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<DomainEntities.Notification> Notifications { get; set; }
    public DbSet<DomainEntitiesInvitation.Invitation> Invitations { get; set; }
    public DbSet<DomainEntitiesInvitation.InvitationType> InvitationTypes { get; set; }
}
