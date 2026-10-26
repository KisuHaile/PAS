namespace Persistence.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }
        IItemMasterRepository ItemMasters { get; }
        IWarehouseRepository Warehouses { get; }
        IInventoryStockRepository InventoryStocks { get; }
        IPropertyRepository Properties { get; }
        IServiceRequestRepository ServiceRequests { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}