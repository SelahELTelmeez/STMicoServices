using Microsoft.EntityFrameworkCore;
using TransactionEntites.Entities.Activities;
using TransactionEntites.Entities.Lessons;

namespace TransactionEntites.Entities
{
    public class studentTrackerDbContext : DbContext
    {
        public studentTrackerDbContext(DbContextOptions<studentTrackerDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<StudentActivityTracker> StudentActivityTracker { get; set; }
        public DbSet<StudentLessonTracker> StudentLessonTracker { get; set; }
    }
}
