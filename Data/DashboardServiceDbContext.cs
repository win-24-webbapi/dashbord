using Microsoft.EntityFrameworkCore;
using DashboardService.API.Models;

namespace DashboardService.API.Data
{
    public class DashboardServiceDbContext : DbContext
    {
        public DashboardServiceDbContext(DbContextOptions<DashboardServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<DashboardStats> DashboardStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DashboardStats>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18,2)");
            });
        }
    }
}