using Domain.Requisition;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Requisition
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.ToTable("ServiceRequests");

            builder.Property(s => s.SRNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(s => s.ApprovalStatus)
                .WithMany()
                .HasForeignKey(s => s.ApprovalStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.SRNumber).IsUnique();
            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => s.ApprovalStatusId);
        }
    }
}