using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Salesperson> Salespersons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Sale entity
            modelBuilder.Entity<Sale>(entity =>
            {
                entity.ToTable("SalesMaster");
                entity.HasKey(e => e.SaleId);
                entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
            });

            // Configure Salesperson entity
            modelBuilder.Entity<Salesperson>(entity =>
            {
                entity.ToTable("Salesperson");
                entity.HasKey(e => e.SalespersonId);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
            });

            // Configure relationships
            modelBuilder.Entity<Sale>()
                .HasOne<Salesperson>()
                .WithMany(s => s.Sales)
                .HasForeignKey(s => s.SalespersonId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
