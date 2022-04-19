using Microsoft.EntityFrameworkCore;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherSubjects;

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
}
