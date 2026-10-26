using Domain.Catalog;
using Domain.Receiving;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Receiving
{
    public class ReceivingNoteItemConfiguration : IEntityTypeConfiguration<ReceivingNoteItem>
    {
        public void Configure(EntityTypeBuilder<ReceivingNoteItem> builder)
        {
            builder.ToTable("ReceivingNoteItems");

            builder.Property(r => r.UnitPrice)
                .HasPrecision(18, 2);

            builder.HasOne(r => r.ReceivingNote)
                .WithMany(n => n.Items)
                .HasForeignKey(r => r.ReceivingNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.ReceivingNoteId, r.ItemId }).IsUnique();
        }
    }
}
