using Microsoft.EntityFrameworkCore;
using StudentEntities.Entities.Rewards;
using StudentEntities.Entities.Trackers;

namespace StudentEntities.Entities;
public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<StudentActivityTracker> StudentActivityTrackers { get; set; }
    public DbSet<StudentQuizTracker> StudentQuizTrackers { get; set; }
    public DbSet<Reward> Rewards { get; set; }
}
