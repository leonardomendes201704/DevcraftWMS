using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ZoneCustomerConfiguration : AuditableEntityConfiguration<ZoneCustomer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ZoneCustomer> builder)
    {
        builder.ToTable("ZoneCustomers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();

        builder.HasOne(x => x.Zone)
            .WithMany(z => z.CustomerAccesses)
            .HasForeignKey(x => x.ZoneId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ZoneId, x.CustomerId }).IsUnique();
        builder.HasIndex(x => x.CustomerId);
    }
}
