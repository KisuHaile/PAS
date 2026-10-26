using Domain.Catalog;
using Domain.Disposal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Disposal
{
    public class DisposalRecordConfiguration : IEntityTypeConfiguration<DisposalRecord>
    {
        public void Configure(EntityTypeBuilder<DisposalRecord> builder)
        {
            builder.ToTable("DisposalRecords");

            builder.Property(d => d.Reason)
                .HasMaxLength(500);

            builder.Property(d => d.EstimatedValue)
                .HasPrecision(18, 2);

            builder.HasOne(d => d.Item)
                .WithMany()
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => d.DisposalDate);
        }
    }
}