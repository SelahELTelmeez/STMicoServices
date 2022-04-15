using Microsoft.EntityFrameworkCore;
using TransactionEntites.Entities.Rewards;
using TransactionEntites.Entities.TeacherClasses;
using TransactionEntites.Entities.TeacherSubjects;
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

    public DbSet<StudentActivityTracker> StudentActivityTrackers { get; set; }
    public DbSet<TeacherClass> TeacherClasses { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<StudentQuizTracker> StudentQuizTrackers { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<DomainEntities.Notification> Notifications { get; set; }
    public DbSet<DomainEntitiesInvitation.Invitation> Invitations { get; set; }
    public DbSet<DomainEntitiesInvitation.InvitationType> InvitationTypes { get; set; }
}
