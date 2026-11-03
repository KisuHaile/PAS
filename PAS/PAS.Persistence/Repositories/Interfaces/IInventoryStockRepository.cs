using Domain.Storage;

namespace Persistence.Repositories.Interfaces
{
    public interface IInventoryStockRepository : IGenericRepository<InventoryStock>
    {
        Task<InventoryStock?> GetStockAsync(Guid itemId, Guid shelfId);
        Task<IEnumerable<InventoryStock>> GetStockByItemAsync(Guid itemId);
        Task<IEnumerable<InventoryStock>> GetStockByShelfAsync(Guid shelfId);
        Task<bool> CheckAvailabilityAsync(Guid itemId, int requestedQuantity);
        Task UpdateStockAsync(Guid itemId, Guid shelfId, int quantityChange, string transactionType, Guid referenceId);
    }
}