using Microsoft.EntityFrameworkCore;
using TransactionEntites.Entities.Rewards;
using TransactionEntites.Entities.Trackers;

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
    public DbSet<StudentQuizTracker> StudentQuizTrackers { get; set; }
    public DbSet<Reward> Rewards { get; set; }
}
