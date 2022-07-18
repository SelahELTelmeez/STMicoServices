using IdentityEntities.Entities.Grades;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Entities.Locations;
using IdentityEntities.Shared.Identities;
using Microsoft.EntityFrameworkCore;

namespace IdentityEntities.Entities;
public class STIdentityDbContext : DbContext
{
    public STIdentityDbContext(DbContextOptions<STIdentityDbContext> options) : base(options)
    {
    }

    public DbSet<Avatar> Avatars { get; set; }
    public DbSet<ExternalIdentityProvider> ExternalIdentityProviders { get; set; }
    public DbSet<IdentityRelation> IdentityRelations { get; set; }
    public DbSet<IdentityRefreshToken> IdentityRefreshTokens { get; set; }
    public DbSet<IdentityRole> IdentityRoles { get; set; }
    public DbSet<IdentitySchool> IdentitySchools { get; set; }
    public DbSet<IdentityUser> IdentityUsers { get; set; }
    public DbSet<IdentityTemporaryValueHolder> IdentityTemporaryValueHolders { get; set; }
    public DbSet<IdentityActivation> IdentityActivations { get; set; }
    public DbSet<IdentityReferralTracker> IdentityReferralTrackers { get; set; }
    public DbSet<Governorate> Governorates { get; set; }
    public DbSet<Grade> Grades { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avatar>().Property(e => e.Id).ValueGeneratedNever();
        modelBuilder.Entity<IdentityRole>().Property(e => e.Id).ValueGeneratedNever();
        base.OnModelCreating(modelBuilder);
    }
}
