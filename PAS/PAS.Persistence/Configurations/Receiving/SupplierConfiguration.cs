using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Receiving;

namespace Persistence.Configurations.Receiving
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.Property(s => s.SupplierName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.ContactPerson)
                .HasMaxLength(100);

            builder.Property(s => s.TinNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(s => s.TinNumber).IsUnique();
            builder.HasIndex(s => s.SupplierName);
        }
    }
}