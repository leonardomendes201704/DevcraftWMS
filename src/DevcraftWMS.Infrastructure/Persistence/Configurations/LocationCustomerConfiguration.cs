using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class LocationCustomerConfiguration : AuditableEntityConfiguration<LocationCustomer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<LocationCustomer> builder)
    {
        builder.ToTable("LocationCustomers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();

        builder.HasOne(x => x.Location)
            .WithMany(l => l.CustomerAccesses)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.LocationId, x.CustomerId }).IsUnique();
        builder.HasIndex(x => x.CustomerId);
    }
}
