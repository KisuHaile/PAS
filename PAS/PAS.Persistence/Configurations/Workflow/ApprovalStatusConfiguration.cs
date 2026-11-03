using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Workflow;

namespace Persistence.Configurations.Workflow
{
    public class ApprovalStatusConfiguration : IEntityTypeConfiguration<ApprovalStatus>
    {
        public void Configure(EntityTypeBuilder<ApprovalStatus> builder)
        {
            builder.ToTable("ApprovalStatuses");

            builder.Property(a => a.StatusName)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(a => a.StatusName).IsUnique();
        }
    }
}