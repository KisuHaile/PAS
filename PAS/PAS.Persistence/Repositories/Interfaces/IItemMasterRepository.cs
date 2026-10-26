using Domain.Catalog;

namespace Persistence.Repositories.Interfaces
{
    public interface IItemMasterRepository : IGenericRepository<ItemMaster>
    {
        Task<ItemMaster?> GetBySKUAsync(string sku);
        Task<IEnumerable<ItemMaster>> GetItemsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<ItemMaster>> GetLowStockItemsAsync(int threshold = 10);
        Task<bool> IsSKUUniqueAsync(string sku, Guid? excludeId = null);
        Task<IEnumerable<ItemMaster>> GetItemsRequiringInspectionAsync();
    }
}