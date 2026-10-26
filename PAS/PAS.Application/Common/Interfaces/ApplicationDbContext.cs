using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Domain.Catalog;
using Domain.Users;
using Domain.Storage;
using Domain.PropertyManagement;
using Domain.Common;
using Domain.Receiving;
using Domain.Requisition;
using Domain.Disposal;
using Domain.TransferReturn;
using Domain.Workflow;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Category> Categories { get; }
        DbSet<ItemMaster> ItemMasters { get; }
        DbSet<Employee> Employees { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<Warehouse> Warehouses { get; }
        DbSet<ShelfLocation> ShelfLocations { get; }
        DbSet<InventoryStock> InventoryStocks { get; }
        DbSet<StockLedger> StockLedgers { get; }
        DbSet<PropertyType> PropertyTypes { get; }
        DbSet<PropertyLocation> PropertyLocations { get; }
        DbSet<PropertyCategory> PropertyCategories { get; }
        DbSet<SafetyBox> SafetyBoxes { get; }
        DbSet<SafetyBoxShelf> SafetyBoxShelves { get; }
        DbSet<Property> Properties { get; }
        DbSet<Supplier> Suppliers { get; }
        DbSet<ReceivingNote> ReceivingNotes { get; }
        DbSet<InspectionLog> InspectionLogs { get; }
        DbSet<ServiceRequest> ServiceRequests { get; }
        DbSet<SR_Detail> SR_Details { get; }
        DbSet<StoreIssueVoucher> StoreIssueVouchers { get; }
        DbSet<DisposalRecord> DisposalRecords { get; }
        DbSet<TransferRecord> TransferRecords { get; }
        DbSet<ReturnMaterialRequestNote> ReturnMaterialRequestNotes { get; }
        DbSet<ApprovalWorkflow> ApprovalWorkflows { get; }
        DbSet<Approver> Approvers { get; }
        DbSet<ApprovalStatus> ApprovalStatuses { get; }
        DbSet<AuditTrail> AuditTrails { get; }
        DbSet<DocumentAttachment> DocumentAttachments { get; }
        DbSet<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}