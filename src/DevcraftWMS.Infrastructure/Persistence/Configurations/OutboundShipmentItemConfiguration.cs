using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class OutboundShipmentItemConfiguration : IEntityTypeConfiguration<OutboundShipmentItem>
{
    public void Configure(EntityTypeBuilder<OutboundShipmentItem> builder)
    {
        builder.ToTable("OutboundShipmentItems");

        builder.Property(x => x.PackageNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.OutboundPackage)
            .WithMany()
            .HasForeignKey(x => x.OutboundPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OutboundShipmentId);
    }
}
