using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class AsnStatusEventConfiguration : IEntityTypeConfiguration<AsnStatusEvent>
{
    public void Configure(EntityTypeBuilder<AsnStatusEvent> builder)
    {
        builder.ToTable("AsnStatusEvents");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.HasIndex(e => e.AsnId);

        builder.HasOne(e => e.Asn)
            .WithMany(a => a.StatusEvents)
            .HasForeignKey(e => e.AsnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
