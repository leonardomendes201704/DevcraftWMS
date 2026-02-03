using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ReceiptDivergenceConfiguration : AuditableEntityConfiguration<ReceiptDivergence>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReceiptDivergence> builder)
    {
        builder.ToTable("ReceiptDivergences");

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.HasOne(x => x.Receipt)
            .WithMany()
            .HasForeignKey(x => x.ReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.InboundOrderItem)
            .WithMany()
            .HasForeignKey(x => x.InboundOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ReceiptId);
        builder.HasIndex(x => x.InboundOrderId);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
