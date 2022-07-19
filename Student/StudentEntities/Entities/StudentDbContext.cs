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

    public DbSet<ActivityTracker> ActivityTrackers { get; set; }
    public DbSet<QuizTracker> QuizTrackers { get; set; }
    public DbSet<Reward> Rewards { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reward>()
       .Property(a => a.StudentId)
       .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<ActivityTracker>()
        .Property(a => a.StudentId)
        .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<QuizTracker>()
         .Property(a => a.StudentId)
         .HasConversion(v => v.ToLower(), v => v.ToLower());

        base.OnModelCreating(modelBuilder);
    }
}
