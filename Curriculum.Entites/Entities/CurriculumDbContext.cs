using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Curriculums;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.MCQS;
using CurriculumEntites.Entities.Units;
using Microsoft.EntityFrameworkCore;

namespace CurriculumEntites.Entities;
public class CurriculumDbContext : DbContext
{
    public CurriculumDbContext(DbContextOptions<CurriculumDbContext> options) : base(options)
    {
    }

    public DbSet<Curriculum> Curriculums { get; set; }
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
        modelBuilder.Entity<CurriculumGroup>().HasData(
            new CurriculumGroup { Id = 1, Name = "KG1" },
            new CurriculumGroup { Id = 2, Name = "KG2" },
            new CurriculumGroup { Id = 3, Name = "الصف الأول الإبتدائى" },
            new CurriculumGroup { Id = 4, Name = "الصف الثانى الإبتدائى" },
            new CurriculumGroup { Id = 5, Name = "الصف الثالث الإبتدائى" },
            new CurriculumGroup { Id = 6, Name = "الصف الرابع الإبتدائى" },
            new CurriculumGroup { Id = 7, Name = "الصف الخامس الإبتدائى" },
            new CurriculumGroup { Id = 8, Name = "الصف السادس الإبتدائى" },
            new CurriculumGroup { Id = 9, Name = "الصف الأول الإعدادى" },
            new CurriculumGroup { Id = 10, Name = "الصف الثانى الإعدادى" },
            new CurriculumGroup { Id = 11, Name = "الصف الثالث الإعدادى" },
            new CurriculumGroup { Id = 12, Name = "الصف الأول الثانوى" },
            new CurriculumGroup { Id = 13, Name = "الصف الثانى الثانوى" },
            new CurriculumGroup { Id = 14, Name = "الثانوية العامة" });

        modelBuilder.Entity<CurriculumLanguage>().HasData(
            new CurriculumLanguage { Id = 1, Name = "عربى" },
            new CurriculumLanguage { Id = 2, Name = "لغات" },
            new CurriculumLanguage { Id = 3, Name = "الكل" });

        base.OnModelCreating(modelBuilder);
    }
}
