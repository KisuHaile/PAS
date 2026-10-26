using Domain.Requisition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Requisition
{
    public class StoreIssueVoucherConfiguration : IEntityTypeConfiguration<StoreIssueVoucher>
    {
        public void Configure(EntityTypeBuilder<StoreIssueVoucher> builder)
        {
            builder.ToTable("StoreIssueVouchers");

            builder.Property(s => s.SIVNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.RecipientSignature)
                .HasMaxLength(500);

            builder.HasOne(s => s.ServiceRequest)
                .WithOne(sr => sr.StoreIssueVoucher)
                .HasForeignKey<StoreIssueVoucher>(s => s.SRId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.SIVNumber).IsUnique();
            builder.HasIndex(s => s.SRId).IsUnique();
        }
    }
}