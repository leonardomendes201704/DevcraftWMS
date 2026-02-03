using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class ReceiptDivergenceEvidenceConfiguration : AuditableEntityConfiguration<ReceiptDivergenceEvidence>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ReceiptDivergenceEvidence> builder)
    {
        builder.ToTable("ReceiptDivergenceEvidence");

        builder.Property(x => x.FileName)
            .HasMaxLength(260)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(x => x.ReceiptDivergence)
            .WithMany(x => x.Evidence)
            .HasForeignKey(x => x.ReceiptDivergenceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ReceiptDivergenceId);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
