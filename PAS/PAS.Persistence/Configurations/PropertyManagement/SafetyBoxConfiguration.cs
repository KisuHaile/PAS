using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PropertyManagement;

namespace Persistence.Configurations.PropertyManagement
{
    public class SafetyBoxConfiguration : IEntityTypeConfiguration<SafetyBox>
    {
        public void Configure(EntityTypeBuilder<SafetyBox> builder)
        {
            builder.ToTable("SafetyBoxes");

            builder.Property(s => s.BoxNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.TotalShelves)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(s => s.Location)
                .WithMany(l => l.SafetyBoxes)
                .HasForeignKey(s => s.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Shelves)
                .WithOne()
                .HasForeignKey(s => s.SafetyBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.BoxNumber).IsUnique();
        }
    }
}