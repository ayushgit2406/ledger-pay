using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status).HasConversion<string>().IsRequired();

            builder.OwnsOne(o => o.Amount, m =>
            {
                m.Property(p => p.Amount).IsRequired();
                m.Property(p => p.Currency).IsRequired();
            });
        }
    }
}