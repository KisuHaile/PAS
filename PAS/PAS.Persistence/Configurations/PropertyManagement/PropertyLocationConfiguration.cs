using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PropertyManagement;

namespace Persistence.Configurations.PropertyManagement
{
    public class PropertyLocationConfiguration : IEntityTypeConfiguration<PropertyLocation>
    {
        public void Configure(EntityTypeBuilder<PropertyLocation> builder)
        {
            builder.ToTable("PropertyLocations");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LocationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.Name).IsUnique();
        }
    }
}