using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Curriculums;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Units;
using Microsoft.EntityFrameworkCore;

namespace CurriculumEntites.Entities;
public class CurriculumDbContext : DbContext
{
    public CurriculumDbContext(DbContextOptions<CurriculumDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Curriculum> Curriculum { get; set; }
    public DbSet<Clip> Clip { get; set; }
    public DbSet<Lesson> Lesson { get; set; }
    public DbSet<Unit> Unit { get; set; }
    public DbSet<CurriculumGroup> CurriculumGroup { get; set; }
    public DbSet<CurriculumLanguage> CurriculumLanguage { get; set; }
}
