using Microsoft.EntityFrameworkCore;
using Domain.Catalog;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class ItemMasterRepository : GenericRepository<ItemMaster>, IItemMasterRepository
    {
        public ItemMasterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ItemMaster?> GetBySKUAsync(string sku)
        {
            return await _dbSet
                .FirstOrDefaultAsync(i => i.SKU == sku);
        }

        public async Task<IEnumerable<ItemMaster>> GetItemsByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Where(i => i.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ItemMaster>> GetLowStockItemsAsync(int threshold = 10)
        {
            var inventoryStocks = await _context.InventoryStocks
                .GroupBy(i => i.ItemId)
                .Select(g => new { ItemId = g.Key, TotalStock = g.Sum(i => i.CurrentQuantity) })
                .ToListAsync();

            var lowStockItems = new List<ItemMaster>();

            foreach (var stock in inventoryStocks)
            {
                var item = await GetByIdAsync(stock.ItemId);
                if (item != null && stock.TotalStock <= item.MinStockLevel)
                {
                    lowStockItems.Add(item);
                }
            }

            return lowStockItems;
        }

        public async Task<bool> IsSKUUniqueAsync(string sku, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(i => i.SKU == sku && i.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(i => i.SKU == sku);
        }

        public async Task<IEnumerable<ItemMaster>> GetItemsRequiringInspectionAsync()
        {
            return await _dbSet
                .Where(i => i.RequiresInspection)
                .ToListAsync();
        }
    }
}