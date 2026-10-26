using Domain.Catalog;
using Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Storage
{
    public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
    {
        public void Configure(EntityTypeBuilder<InventoryStock> builder)
        {
            builder.ToTable("InventoryStocks");

            builder.Property(i => i.CurrentQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(i => i.ReservedQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(i => i.Item)
                .WithMany()
                .HasForeignKey(i => i.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.ShelfLocation)
                .WithMany()
                .HasForeignKey(i => i.ShelfId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => new { i.ItemId, i.ShelfId }).IsUnique();
        }
    }
}