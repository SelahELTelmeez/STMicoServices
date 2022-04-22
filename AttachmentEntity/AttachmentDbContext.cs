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

    public DbSet<Attachment> Attachments { get; set; }
}
