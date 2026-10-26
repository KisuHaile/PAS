using Application.Common.Interfaces;
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
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace PAS.API.Services;

public class ApplicationDbContextAdapter : IApplicationDbContext
{
    private readonly ApplicationDbContext _db;

    public ApplicationDbContextAdapter(ApplicationDbContext db)
    {
        _db = db;
    }

    public DbSet<Category> Categories => _db.Set<Category>();
    public DbSet<ItemMaster> ItemMasters => _db.Set<ItemMaster>();
    public DbSet<Employee> Employees => _db.Set<Employee>();
    public DbSet<Permission> Permissions => _db.Set<Permission>();
    public DbSet<Warehouse> Warehouses => _db.Set<Warehouse>();
    public DbSet<ShelfLocation> ShelfLocations => _db.Set<ShelfLocation>();
    public DbSet<InventoryStock> InventoryStocks => _db.Set<InventoryStock>();
    public DbSet<StockLedger> StockLedgers => _db.Set<StockLedger>();
    public DbSet<PropertyType> PropertyTypes => _db.Set<PropertyType>();
    public DbSet<PropertyLocation> PropertyLocations => _db.Set<PropertyLocation>();
    public DbSet<PropertyCategory> PropertyCategories => _db.Set<PropertyCategory>();
    public DbSet<SafetyBox> SafetyBoxes => _db.Set<SafetyBox>();
    public DbSet<SafetyBoxShelf> SafetyBoxShelves => _db.Set<SafetyBoxShelf>();
    public DbSet<Property> Properties => _db.Set<Property>();
    public DbSet<Supplier> Suppliers => _db.Set<Supplier>();
    public DbSet<ReceivingNote> ReceivingNotes => _db.Set<ReceivingNote>();
    public DbSet<InspectionLog> InspectionLogs => _db.Set<InspectionLog>();
    public DbSet<ServiceRequest> ServiceRequests => _db.Set<ServiceRequest>();
    public DbSet<SR_Detail> SR_Details => _db.Set<SR_Detail>();
    public DbSet<StoreIssueVoucher> StoreIssueVouchers => _db.Set<StoreIssueVoucher>();
    public DbSet<DisposalRecord> DisposalRecords => _db.Set<DisposalRecord>();
    public DbSet<TransferRecord> TransferRecords => _db.Set<TransferRecord>();
    public DbSet<ReturnMaterialRequestNote> ReturnMaterialRequestNotes => _db.Set<ReturnMaterialRequestNote>();
    public DbSet<ApprovalWorkflow> ApprovalWorkflows => _db.Set<ApprovalWorkflow>();
    public DbSet<Approver> Approvers => _db.Set<Approver>();
    public DbSet<ApprovalStatus> ApprovalStatuses => _db.Set<ApprovalStatus>();
    public DbSet<AuditTrail> AuditTrails => _db.Set<AuditTrail>();
    public DbSet<DocumentAttachment> DocumentAttachments => _db.Set<DocumentAttachment>();
    public DbSet<Notification> Notifications => _db.Set<Notification>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
