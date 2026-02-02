using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class AisleCustomerConfiguration : AuditableEntityConfiguration<AisleCustomer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AisleCustomer> builder)
    {
        builder.ToTable("AisleCustomers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();

        builder.HasOne(x => x.Aisle)
            .WithMany(a => a.CustomerAccesses)
            .HasForeignKey(x => x.AisleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.AisleId, x.CustomerId }).IsUnique();
        builder.HasIndex(x => x.CustomerId);
    }
}
