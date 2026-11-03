using Microsoft.EntityFrameworkCore;
using Domain.Storage;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class InventoryStockRepository : GenericRepository<InventoryStock>, IInventoryStockRepository
    {
        public InventoryStockRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<InventoryStock?> GetStockAsync(Guid itemId, Guid shelfId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.ItemId == itemId && s.ShelfId == shelfId);
        }

        public async Task<IEnumerable<InventoryStock>> GetStockByItemAsync(Guid itemId)
        {
            return await _dbSet
                .Where(s => s.ItemId == itemId)
                .Include(s => s.ShelfLocation)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryStock>> GetStockByShelfAsync(Guid shelfId)
        {
            return await _dbSet
                .Where(s => s.ShelfId == shelfId)
                .Include(s => s.Item)
                .ToListAsync();
        }

        public async Task<bool> CheckAvailabilityAsync(Guid itemId, int requestedQuantity)
        {
            var totalAvailable = await _dbSet
                .Where(s => s.ItemId == itemId)
                .SumAsync(s => s.CurrentQuantity - s.ReservedQuantity);

            return totalAvailable >= requestedQuantity;
        }

        public async Task UpdateStockAsync(Guid itemId, Guid shelfId, int quantityChange, string transactionType, Guid referenceId)
        {
            var stock = await GetStockAsync(itemId, shelfId);

            if (stock == null && quantityChange > 0)
            {
                stock = new InventoryStock(itemId, shelfId, quantityChange);
                await AddAsync(stock);
            }
            else if (stock != null)
            {
                // Update current quantity
                var currentQuantity = stock.CurrentQuantity + quantityChange;

                // Use reflection to update private field (or add method to domain entity)
                typeof(InventoryStock).GetProperty("CurrentQuantity")?
                    .SetValue(stock, Math.Max(0, currentQuantity));

                Update(stock);
            }

            // Create ledger entry
            var ledger = new StockLedger(itemId, shelfId, quantityChange, transactionType, referenceId);
            await _context.StockLedgers.AddAsync(ledger);
        }
    }
}