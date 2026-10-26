using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public ICategoryRepository Categories { get; }
        public IItemMasterRepository ItemMasters { get; }
        public IWarehouseRepository Warehouses { get; }
        public IInventoryStockRepository InventoryStocks { get; }
        public IPropertyRepository Properties { get; }
        public IServiceRequestRepository ServiceRequests { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            ICategoryRepository categoryRepository,
            IItemMasterRepository itemMasterRepository,
            IWarehouseRepository warehouseRepository,
            IInventoryStockRepository inventoryStockRepository,
            IPropertyRepository propertyRepository,
            IServiceRequestRepository serviceRequestRepository)
        {
            _context = context;
            Categories = categoryRepository;
            ItemMasters = itemMasterRepository;
            Warehouses = warehouseRepository;
            InventoryStocks = inventoryStockRepository;
            Properties = propertyRepository;
            ServiceRequests = serviceRequestRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await (_transaction?.CommitAsync() ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            await (_transaction?.RollbackAsync() ?? Task.CompletedTask);
            _transaction?.Dispose();
            _transaction = null;
        }

        private async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}