using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ReceiptCountConfiguration : AuditableEntityConfiguration<ReceiptCount>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReceiptCount> builder)
    {
        builder.ToTable("ReceiptCounts");
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.ReceiptId).IsRequired();
        builder.Property(rc => rc.InboundOrderItemId).IsRequired();
        builder.Property(rc => rc.ExpectedQuantity).HasColumnType("decimal(18,3)");
        builder.Property(rc => rc.CountedQuantity).HasColumnType("decimal(18,3)");
        builder.Property(rc => rc.Variance).HasColumnType("decimal(18,3)");
        builder.Property(rc => rc.Mode).IsRequired();
        builder.Property(rc => rc.Notes).HasMaxLength(500);

        builder.HasOne(rc => rc.Receipt)
            .WithMany()
            .HasForeignKey(rc => rc.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rc => rc.InboundOrderItem)
            .WithMany()
            .HasForeignKey(rc => rc.InboundOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(rc => rc.ReceiptId);
        builder.HasIndex(rc => rc.InboundOrderItemId);
        builder.HasIndex(rc => rc.Mode);
        builder.HasIndex(rc => rc.Variance);
        builder.HasIndex(rc => new { rc.ReceiptId, rc.InboundOrderItemId }).IsUnique();
    }
}
