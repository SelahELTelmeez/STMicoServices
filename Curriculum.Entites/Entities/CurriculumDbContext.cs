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
    public DbSet<Curriculum> Curriculums { get; set; }
    public DbSet<Clip> Clips { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<CurriculumGroup> CurriculumGroups { get; set; }
    public DbSet<CurriculumLanguage> CurriculumLanguages { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
