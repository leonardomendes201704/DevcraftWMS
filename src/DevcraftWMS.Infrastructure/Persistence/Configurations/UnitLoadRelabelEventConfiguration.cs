using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class UnitLoadRelabelEventConfiguration : IEntityTypeConfiguration<UnitLoadRelabelEvent>
{
    public void Configure(EntityTypeBuilder<UnitLoadRelabelEvent> builder)
    {
        builder.ToTable("UnitLoadRelabelEvents");

        builder.Property(x => x.OldSsccInternal)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NewSsccInternal)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasIndex(x => x.UnitLoadId);
        builder.HasIndex(x => x.RelabeledAtUtc);

        builder.HasOne(x => x.UnitLoad)
            .WithMany(x => x.RelabelHistory)
            .HasForeignKey(x => x.UnitLoadId);
    }
}
