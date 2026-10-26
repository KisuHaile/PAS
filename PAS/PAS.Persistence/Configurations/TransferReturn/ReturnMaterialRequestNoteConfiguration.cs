using Domain.TransferReturn;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.TransferReturn
{
    public class ReturnMaterialRequestNoteConfiguration : IEntityTypeConfiguration<ReturnMaterialRequestNote>
    {
        public void Configure(EntityTypeBuilder<ReturnMaterialRequestNote> builder)
        {
            builder.ToTable("ReturnMaterialRequestNotes");

            builder.Property(r => r.Reason)
                .HasMaxLength(500);

            builder.HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.RequestDate);
        }
    }
}