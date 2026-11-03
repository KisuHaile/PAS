using Domain.Catalog;
using Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Storage
{
    public class StockLedgerConfiguration : IEntityTypeConfiguration<StockLedger>
    {
        public void Configure(EntityTypeBuilder<StockLedger> builder)
        {
            builder.ToTable("StockLedgers");

            builder.Property(s => s.TransactionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(s => s.Item)
                .WithMany()
                .HasForeignKey(s => s.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Shelf)
                .WithMany()
                .HasForeignKey(s => s.ShelfId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.ReferenceId);
            builder.HasIndex(s => s.CreatedDate);
        }
    }
}