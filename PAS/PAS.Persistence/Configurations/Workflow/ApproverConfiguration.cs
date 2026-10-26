using Domain.Workflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations.Workflow
{
    public class ApproverConfiguration : IEntityTypeConfiguration<Approver>
    {
        public void Configure(EntityTypeBuilder<Approver> builder)
        {
            builder.ToTable("Approvers");

            builder.HasOne(a => a.Workflow)
                .WithMany(w => w.Approvers)
                .HasForeignKey(a => a.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => new { a.WorkflowId, a.ApprovalLevel }).IsUnique();
        }
    }
}