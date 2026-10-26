using Domain.Catalog;
using Domain.Common;
using Domain.Disposal;
using Domain.PropertyManagement;
using Domain.Receiving;
using Domain.Requisition;
using Domain.Storage;
using Domain.TransferReturn;
using Domain.Users;
using Domain.Workflow;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Identity;

namespace Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Catalog
        public DbSet<Category> Categories { get; set; }
        public DbSet<ItemMaster> ItemMasters { get; set; }

        // Storage
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ShelfLocation> ShelfLocations { get; set; }
        public DbSet<InventoryStock> InventoryStocks { get; set; }
        public DbSet<StockLedger> StockLedgers { get; set; }

        // Property
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyCategory> PropertyCategories { get; set; }

        // Requisition
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<SR_Detail> SR_Details { get; set; }
        public DbSet<StoreIssueVoucher> StoreIssueVouchers { get; set; }

        // Receiving
        public DbSet<ReceivingNote> ReceivingNotes { get; set; }
        public DbSet<InspectionLog> InspectionLogs { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        // Disposal
        public DbSet<DisposalRecord> DisposalRecords { get; set; }

        // Transfer/Return
        public DbSet<ReturnMaterialRequestNote> ReturnMaterialRequestNotes { get; set; }
        public DbSet<TransferRecord> TransferRecords { get; set; }

        // Common
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Users
        public DbSet<Employee> Employees { get; set; }

        // Workflow
        public DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }
        public DbSet<Approver> Approvers { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly
            );
        }
    }
}