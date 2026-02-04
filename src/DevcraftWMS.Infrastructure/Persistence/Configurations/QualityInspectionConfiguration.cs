using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class QualityInspectionConfiguration : AuditableEntityConfiguration<QualityInspection>
{
    protected override void ConfigureEntity(EntityTypeBuilder<QualityInspection> builder)
    {
        builder.ToTable("QualityInspections");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.CustomerId).IsRequired();
        builder.Property(q => q.WarehouseId).IsRequired();
        builder.Property(q => q.ReceiptId).IsRequired();
        builder.Property(q => q.ProductId).IsRequired();
        builder.Property(q => q.LocationId).IsRequired();
        builder.Property(q => q.Status).IsRequired();
        builder.Property(q => q.Reason).HasMaxLength(200).IsRequired();
        builder.Property(q => q.Notes).HasMaxLength(1000);
        builder.Property(q => q.DecisionNotes).HasMaxLength(1000);
        builder.Property(q => q.DecisionByUserId);

        builder.HasOne(q => q.Receipt)
            .WithMany()
            .HasForeignKey(q => q.ReceiptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.ReceiptItem)
            .WithMany()
            .HasForeignKey(q => q.ReceiptItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Product)
            .WithMany()
            .HasForeignKey(q => q.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Lot)
            .WithMany()
            .HasForeignKey(q => q.LotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Location)
            .WithMany()
            .HasForeignKey(q => q.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(q => q.Status);
        builder.HasIndex(q => q.CreatedAtUtc);
        builder.HasIndex(q => q.LotId);
        builder.HasIndex(q => q.ProductId);
        builder.HasIndex(q => q.ReceiptId);
    }
}
