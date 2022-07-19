using AttachmentEntities.Entities.Attachments;
using Microsoft.EntityFrameworkCore;

namespace AttachmentEntity;

public class AttachmentDbContext : DbContext
{
    public AttachmentDbContext(DbContextOptions<AttachmentDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Attachment> Attachments { get; set; }
}
