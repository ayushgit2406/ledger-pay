using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class LedgerPayDbContext : DbContext
    {
        public LedgerPayDbContext(DbContextOptions<LedgerPayDbContext> options) : base(options)
        { }
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LedgerPayDbContext).Assembly);
        }
    }
}