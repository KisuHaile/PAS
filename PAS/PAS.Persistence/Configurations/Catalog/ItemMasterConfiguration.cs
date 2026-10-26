using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Catalog;

namespace Persistence.Configurations.Catalog
{
    public class ItemMasterConfiguration : IEntityTypeConfiguration<ItemMaster>
    {
        public void Configure(EntityTypeBuilder<ItemMaster> builder)
        {
            builder.ToTable("ItemMasters");

            builder.Property(i => i.SKU)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(i => i.ItemName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.UnitOfMeasure)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(i => i.RequiresInspection)
                .HasDefaultValue(false);

            builder.Property(i => i.MinStockLevel)
                .HasDefaultValue(0);

            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            builder.HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.SKU).IsUnique();
            builder.HasIndex(i => i.ItemName);
            builder.HasIndex(i => i.CategoryId);
        }
    }
}