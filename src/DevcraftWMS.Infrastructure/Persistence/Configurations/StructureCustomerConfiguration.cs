using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class StructureCustomerConfiguration : AuditableEntityConfiguration<StructureCustomer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<StructureCustomer> builder)
    {
        builder.ToTable("StructureCustomers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();

        builder.HasOne(x => x.Structure)
            .WithMany(s => s.CustomerAccesses)
            .HasForeignKey(x => x.StructureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.StructureId, x.CustomerId }).IsUnique();
        builder.HasIndex(x => x.CustomerId);
    }
}
