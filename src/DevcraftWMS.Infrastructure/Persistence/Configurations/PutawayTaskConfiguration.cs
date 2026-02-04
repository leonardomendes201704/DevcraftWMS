using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class PutawayTaskConfiguration : AuditableEntityConfiguration<PutawayTask>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PutawayTask> builder)
    {
        builder.ToTable("PutawayTasks");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.ReceiptId);
        builder.HasIndex(x => x.UnitLoadId)
            .IsUnique();
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.UnitLoad)
            .WithMany()
            .HasForeignKey(x => x.UnitLoadId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receipt)
            .WithMany()
            .HasForeignKey(x => x.ReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
