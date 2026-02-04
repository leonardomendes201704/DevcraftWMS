using DevcraftWMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevcraftWMS.Infrastructure.Persistence.Configurations;

public sealed class PutawayTaskAssignmentEventConfiguration : AuditableEntityConfiguration<PutawayTaskAssignmentEvent>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PutawayTaskAssignmentEvent> builder)
    {
        builder.ToTable("PutawayTaskAssignmentEvents");

        builder.Property(x => x.FromUserEmail)
            .HasMaxLength(256);

        builder.Property(x => x.ToUserEmail)
            .HasMaxLength(256);

        builder.Property(x => x.Reason)
            .HasMaxLength(512)
            .IsRequired();

        builder.HasIndex(x => x.PutawayTaskId);
        builder.HasIndex(x => x.AssignedAtUtc);
    }
}
