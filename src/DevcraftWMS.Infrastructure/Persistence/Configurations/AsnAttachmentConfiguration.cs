using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DevcraftWMS.Domain.Entities;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class AsnAttachmentConfiguration : IEntityTypeConfiguration<AsnAttachment>
{
    public void Configure(EntityTypeBuilder<AsnAttachment> builder)
    {
        builder.ToTable("AsnAttachments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.ContentType)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.StorageProvider)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasMaxLength(512);

        builder.Property(x => x.StorageUrl)
            .HasMaxLength(1024);

        builder.Property(x => x.ContentBase64);

        builder.Property(x => x.ContentHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.AsnId);

        builder.HasOne(x => x.Asn)
            .WithMany(a => a.Attachments)
            .HasForeignKey(x => x.AsnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
