using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundCheckEvidenceConfiguration : IEntityTypeConfiguration<OutboundCheckEvidence>
{
    public void Configure(EntityTypeBuilder<OutboundCheckEvidence> builder)
    {
        builder.ToTable("OutboundCheckEvidence");

        builder.HasOne(x => x.OutboundCheckItem)
            .WithMany(i => i.Evidence)
            .HasForeignKey(x => x.OutboundCheckItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OutboundCheckItemId);
    }
}
