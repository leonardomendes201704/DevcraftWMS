using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class InboundOrderStatusEventConfiguration : IEntityTypeConfiguration<InboundOrderStatusEvent>
{
    public void Configure(EntityTypeBuilder<InboundOrderStatusEvent> builder)
    {
        builder.ToTable("InboundOrderStatusEvents");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.HasIndex(e => e.InboundOrderId);

        builder.HasOne(e => e.InboundOrder)
            .WithMany(o => o.StatusEvents)
            .HasForeignKey(e => e.InboundOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
