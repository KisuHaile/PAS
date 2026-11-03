using Domain.Receiving;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Receiving
{
    public class ReceivingNoteConfiguration : IEntityTypeConfiguration<ReceivingNote>
    {
        public void Configure(EntityTypeBuilder<ReceivingNote> builder)
        {
            builder.ToTable("ReceivingNotes");

            builder.Property(r => r.GRNNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(r => r.Supplier)
                .WithMany(s => s.ReceivingNotes)
                .HasForeignKey(r => r.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.GRNNumber).IsUnique();
            builder.HasIndex(r => r.Status);
        }
    }
}