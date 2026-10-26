using Domain.Catalog;
using Domain.Requisition;
using Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Requisition
{
    public class SRDetailConfiguration : IEntityTypeConfiguration<SR_Detail>
    {
        public void Configure(EntityTypeBuilder<SR_Detail> builder)
        {
            builder.ToTable("SR_Details");

            builder.Property(s => s.RequestedQty)
                .IsRequired();

            builder.Property(s => s.IssuedQty)
                .HasDefaultValue(0);

            builder.HasOne<ServiceRequest>()
                .WithMany(s => s.Details)
                .HasForeignKey(s => s.SRId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.Item)
                .WithMany()
                .HasForeignKey(s => s.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.ShelfLocation)
                .WithMany()
                .HasForeignKey(s => s.ShelfId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}