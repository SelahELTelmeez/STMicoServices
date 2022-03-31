using Microsoft.EntityFrameworkCore;
using TransactionEntites.Entities.Trackers;

namespace TransactionEntites.Entities;

public class StudentTrackerDbContext : DbContext
{
    public StudentTrackerDbContext(DbContextOptions<StudentTrackerDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<StudentActivityTracker> StudentActivityTracker { get; set; }
    public DbSet<StudentLessonTracker> StudentLessonTracker { get; set; }
}
