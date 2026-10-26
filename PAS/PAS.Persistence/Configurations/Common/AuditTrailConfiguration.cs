using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Common
{
    public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
    {
        public void Configure(EntityTypeBuilder<AuditTrail> builder)
        {
            builder.ToTable("AuditTrails");

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.ActionDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(a => a.EntityId);
            builder.HasIndex(a => a.ActionDate);
            builder.HasIndex(a => new { a.EntityName, a.EntityId });
        }
    }
}