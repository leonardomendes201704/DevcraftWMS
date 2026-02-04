using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class QualityInspectionEvidenceConfiguration : AuditableEntityConfiguration<QualityInspectionEvidence>
{
    protected override void ConfigureEntity(EntityTypeBuilder<QualityInspectionEvidence> builder)
    {
        builder.ToTable("QualityInspectionEvidence");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.QualityInspectionId).IsRequired();
        builder.Property(e => e.FileName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(e => e.SizeBytes).IsRequired();
        builder.Property(e => e.Content).IsRequired();

        builder.HasOne(e => e.QualityInspection)
            .WithMany(q => q.Evidence)
            .HasForeignKey(e => e.QualityInspectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.QualityInspectionId);
    }
}
