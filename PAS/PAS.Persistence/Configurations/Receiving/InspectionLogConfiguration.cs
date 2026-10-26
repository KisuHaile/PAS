using Domain.Receiving;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Receiving
{
    public class InspectionLogConfiguration : IEntityTypeConfiguration<InspectionLog>
    {
        public void Configure(EntityTypeBuilder<InspectionLog> builder)
        {
            builder.ToTable("InspectionLogs");

            builder.Property(i => i.DeviationNotes)
                .HasMaxLength(1000);

            builder.HasOne(i => i.ReceivingNote)
                .WithOne(r => r.InspectionLog)
                .HasForeignKey<InspectionLog>(i => i.ReceivingNoteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.ReceivingNoteId).IsUnique();
        }
    }
}