using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Storage;

namespace Persistence.Configurations.Storage
{
    public class ShelfLocationConfiguration : IEntityTypeConfiguration<ShelfLocation>
    {
        public void Configure(EntityTypeBuilder<ShelfLocation> builder)
        {
            builder.ToTable("ShelfLocations");

            builder.Property(s => s.Aisle)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Rack)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.ShelfNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.QRCodeValue)
                .HasMaxLength(200);

            builder.HasOne(s => s.Warehouse)
                .WithMany(w => w.Shelves)
                .HasForeignKey(s => s.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => new { s.WarehouseId, s.Aisle, s.Rack, s.ShelfNumber })
                .IsUnique();
        }
    }
}