using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Storage;

namespace Persistence.Configurations.Storage
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.Ignore(w => w.ShelfLocations);

            builder.Property(w => w.WarehouseName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.LocationCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(w => w.WarehouseName).IsUnique();
            builder.HasIndex(w => w.LocationCode).IsUnique();
        }
    }
}