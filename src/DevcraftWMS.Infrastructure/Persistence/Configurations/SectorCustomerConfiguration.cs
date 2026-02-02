using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class SectorCustomerConfiguration : AuditableEntityConfiguration<SectorCustomer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SectorCustomer> builder)
    {
        builder.ToTable("SectorCustomers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();

        builder.HasOne(x => x.Sector)
            .WithMany(s => s.CustomerAccesses)
            .HasForeignKey(x => x.SectorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SectorId, x.CustomerId }).IsUnique();
        builder.HasIndex(x => x.CustomerId);
    }
}
