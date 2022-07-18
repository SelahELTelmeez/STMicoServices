using Microsoft.EntityFrameworkCore;

namespace DashboardEntity.Entities;

public class DashboardDbContext : DbContext
{
    public DashboardDbContext(DbContextOptions<DashboardDbContext> options) : base(options)
    {
    }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<SectionGroup> SectionGroups { get; set; }
    public DbSet<Section> Sections { get; set; }
}
