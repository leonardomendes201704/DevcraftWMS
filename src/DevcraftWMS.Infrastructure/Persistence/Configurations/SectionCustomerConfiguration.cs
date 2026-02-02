using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class SectionCustomerConfiguration : AuditableEntityConfiguration<SectionCustomer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SectionCustomer> builder)
    {
        builder.ToTable("SectionCustomers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();

        builder.HasOne(x => x.Section)
            .WithMany(s => s.CustomerAccesses)
            .HasForeignKey(x => x.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SectionId, x.CustomerId }).IsUnique();
        builder.HasIndex(x => x.CustomerId);
    }
}
