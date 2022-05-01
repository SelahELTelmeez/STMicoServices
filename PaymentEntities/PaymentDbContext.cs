using Microsoft.EntityFrameworkCore;
using PaymentEntities.Entities;

namespace PaymentEntities
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<MobileOperator> MobileOperators { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Promocode> Promocodes { get; set; }
        public DbSet<PurchaseContract> PurchaseContracts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MobileOperator>().HasData(
            new MobileOperator { Id = 1, Name = "Orange", IsActive = true, MCC = "602", MNC = "01" },
            new MobileOperator { Id = 2, Name = "Vodafone", IsActive = true, MCC = "602", MNC = "02" },
            new MobileOperator { Id = 3, Name = "Etisalat", IsActive = true, MCC = "602", MNC = "03" },
            new MobileOperator { Id = 4, Name = "WE", IsActive = true, MCC = "602", MNC = "04" });

            base.OnModelCreating(modelBuilder);

        }
    }
}
