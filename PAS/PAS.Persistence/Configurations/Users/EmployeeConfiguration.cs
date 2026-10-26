using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Users;

namespace Persistence.Configurations.Users
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.Property(e => e.EmployeeCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Department)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.EmployeeCode).IsUnique();
        }
    }
}