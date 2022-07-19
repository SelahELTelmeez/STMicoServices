using ChatEntities.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatEntities;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
        .Property(a => a.ToId)
        .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<Message>()
          .Property(a => a.FromId)
          .HasConversion(v => v.ToLower(), v => v.ToLower());

        modelBuilder.Entity<ChatSession>()
        .Property(a => a.UserId)
        .HasConversion(v => v.ToLower(), v => v.ToLower());

        base.OnModelCreating(modelBuilder);
    }
}
