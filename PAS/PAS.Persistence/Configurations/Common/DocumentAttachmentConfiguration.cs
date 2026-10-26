using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Common
{
    public class DocumentAttachmentConfiguration : IEntityTypeConfiguration<DocumentAttachment>
    {
        public void Configure(EntityTypeBuilder<DocumentAttachment> builder)
        {
            builder.ToTable("DocumentAttachments");

            builder.Property(d => d.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(d => d.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.RelatedEntityName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(d => d.ServiceRequest)
                .WithMany()
                .HasForeignKey(d => d.ServiceRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(d => new { d.RelatedEntityId, d.RelatedEntityName });
            builder.HasIndex(d => d.FileName);
            builder.HasIndex(d => d.ServiceRequestId);
        }
    }
}