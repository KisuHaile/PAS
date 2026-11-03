using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Workflow;

namespace Persistence.Configurations.Workflow
{
    public class ApprovalWorkflowConfiguration : IEntityTypeConfiguration<ApprovalWorkflow>
    {
        public void Configure(EntityTypeBuilder<ApprovalWorkflow> builder)
        {
            builder.ToTable("ApprovalWorkflows");

            builder.Property(a => a.WorkflowName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(500);

            builder.HasIndex(a => a.WorkflowName).IsUnique();
        }
    }
}