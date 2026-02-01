using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerfectApiTemplate.Domain.Entities;

namespace PerfectApiTemplate.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : AuditableEntityConfiguration<Customer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(c => c.Email)
            .HasMaxLength(320)
            .IsRequired();
        builder.Property(c => c.DateOfBirth)
            .IsRequired();
        builder.HasIndex(c => c.Email).IsUnique();
    }
}
