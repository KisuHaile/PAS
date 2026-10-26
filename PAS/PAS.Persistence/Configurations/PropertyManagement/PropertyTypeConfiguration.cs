using Domain.PropertyManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.PropertyManagement
{
    public class PropertyTypeConfiguration : IEntityTypeConfiguration<PropertyType>
    {
        public void Configure(EntityTypeBuilder<PropertyType> builder)
        {
            builder.ToTable("PropertyTypes");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

        builder.Ignore(p => p.PropertyCategoryId);
        builder.Ignore(p => p.PropertyCategory);

        builder.HasIndex(p => p.Name).IsUnique();
        }
    }
}