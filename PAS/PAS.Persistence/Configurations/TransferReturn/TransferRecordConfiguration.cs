using Domain.Catalog;
using Domain.PropertyManagement;
using Domain.TransferReturn;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.TransferReturn
{
    public class TransferRecordConfiguration : IEntityTypeConfiguration<TransferRecord>
    {
        public void Configure(EntityTypeBuilder<TransferRecord> builder)
        {
            builder.ToTable("TransferRecords");

            builder.HasOne(t => t.Item)
                .WithMany()
                .HasForeignKey(t => t.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.FromLocation)
                .WithMany()
                .HasForeignKey(t => t.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ToLocation)
                .WithMany()
                .HasForeignKey(t => t.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.TransferDate);
        }
    }
}