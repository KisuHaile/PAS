using Domain.PropertyManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.PropertyManagement
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Properties");

            builder.Property(p => p.TagNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.SerialNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Location)
                .WithMany(l => l.Properties)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.SafetyBox)
                .WithMany(sb => sb.Properties)
                .HasForeignKey(p => p.SafetyBoxId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(p => p.TagNumber).IsUnique();
            builder.HasIndex(p => p.SerialNumber);
            builder.HasIndex(p => p.LocationId);
        }
    }
}