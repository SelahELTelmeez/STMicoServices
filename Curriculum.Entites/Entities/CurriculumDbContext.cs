using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.MCQS;
using CurriculumEntites.Entities.Subjects;
using CurriculumEntites.Entities.Units;
using Microsoft.EntityFrameworkCore;

namespace CurriculumEntites.Entities;
public class CurriculumDbContext : DbContext
{
    public CurriculumDbContext(DbContextOptions<CurriculumDbContext> options) : base(options)
    {
    }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Clip> Clips { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<MCQ> MCQ { get; set; }
    public DbSet<MCQQuestion> MCQQuestion { get; set; }
    public DbSet<MCQAnswer> MCQAnswer { get; set; }
    // public DbSet<StudentActivityRecord> StudentActivityRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SubjectGroup>().HasData(
            new SubjectGroup { Id = 1, Name = "KG1" },
            new SubjectGroup { Id = 2, Name = "KG2" },
            new SubjectGroup { Id = 3, Name = "الصف الأول الإبتدائى" },
            new SubjectGroup { Id = 4, Name = "الصف الثانى الإبتدائى" },
            new SubjectGroup { Id = 5, Name = "الصف الثالث الإبتدائى" },
            new SubjectGroup { Id = 6, Name = "الصف الرابع الإبتدائى" },
            new SubjectGroup { Id = 7, Name = "الصف الخامس الإبتدائى" },
            new SubjectGroup { Id = 8, Name = "الصف السادس الإبتدائى" },
            new SubjectGroup { Id = 9, Name = "الصف الأول الإعدادى" },
            new SubjectGroup { Id = 10, Name = "الصف الثانى الإعدادى" },
            new SubjectGroup { Id = 11, Name = "الصف الثالث الإعدادى" },
            new SubjectGroup { Id = 12, Name = "الصف الأول الثانوى" },
            new SubjectGroup { Id = 13, Name = "الصف الثانى الثانوى" },
            new SubjectGroup { Id = 14, Name = "الثانوية العامة" });

        modelBuilder.Entity<SubjectLanguage>().HasData(
            new SubjectLanguage { Id = 1, Name = "عربى" },
            new SubjectLanguage { Id = 2, Name = "لغات" },
            new SubjectLanguage { Id = 3, Name = "الكل" });

        base.OnModelCreating(modelBuilder);
    }
}
