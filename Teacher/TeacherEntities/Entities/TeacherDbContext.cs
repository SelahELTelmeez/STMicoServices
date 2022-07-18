using Microsoft.EntityFrameworkCore;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherSubjects;
using TeacherEntities.Entities.Trackers;

namespace TeacherEntities.Entities;
public class TeacherDbContext : DbContext
{
    public TeacherDbContext(DbContextOptions<TeacherDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public DbSet<TeacherClass> TeacherClasses { get; set; }
    public DbSet<TeacherSubject> TeacherSubjects { get; set; }
    public DbSet<TeacherAssignment> TeacherAssignments { get; set; }
    public DbSet<TeacherQuiz> TeacherQuizzes { get; set; }
    public DbSet<ClassEnrollee> ClassEnrollees { get; set; }
    public DbSet<TeacherAssignmentActivityTracker> TeacherAssignmentActivityTrackers { get; set; }
    public DbSet<TeacherQuizActivityTracker> TeacherQuizActivityTrackers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeacherClass>()
            .HasMany(left => left.TeacherAssignments)
            .WithMany(right => right.TeacherClasses)
            .UsingEntity(join => join.ToTable("TeacherClassTeacherAssignment"));

        base.OnModelCreating(modelBuilder);
    }
}
