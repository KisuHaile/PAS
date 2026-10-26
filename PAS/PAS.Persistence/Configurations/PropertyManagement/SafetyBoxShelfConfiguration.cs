using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.PropertyManagement;

namespace Persistence.Configurations.PropertyManagement
{
    public class SafetyBoxShelfConfiguration : IEntityTypeConfiguration<SafetyBoxShelf>
    {
        public void Configure(EntityTypeBuilder<SafetyBoxShelf> builder)
        {
            builder.ToTable("SafetyBoxShelves");

            builder.HasOne<SafetyBox>()
                .WithMany(s => s.Shelves)
                .HasForeignKey(s => s.SafetyBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new { s.SafetyBoxId, s.ShelfNumber }).IsUnique();
        }
    }
}