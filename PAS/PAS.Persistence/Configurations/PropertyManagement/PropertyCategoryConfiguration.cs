using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PropertyManagement;

namespace Persistence.Configurations.PropertyManagement
{
    public class PropertyCategoryConfiguration : IEntityTypeConfiguration<PropertyCategory>
    {
        public void Configure(EntityTypeBuilder<PropertyCategory> builder)
        {
            builder.ToTable("PropertyCategories");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.HasIndex(p => p.Name).IsUnique();
        }
    }
}