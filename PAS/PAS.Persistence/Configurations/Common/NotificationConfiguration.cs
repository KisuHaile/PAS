using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Common
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.SentDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => n.IsRead);
            builder.HasIndex(n => n.SentDate);
        }
    }
}