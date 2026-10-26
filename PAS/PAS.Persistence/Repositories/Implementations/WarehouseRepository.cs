using Microsoft.EntityFrameworkCore;
using Domain.Storage;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Warehouse?> GetWarehouseWithShelvesAsync(Guid id)
        {
            return await _dbSet
                .Include(w => w.Shelves)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> IsWarehouseNameUniqueAsync(string name, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(w => w.WarehouseName == name && w.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(w => w.WarehouseName == name);
        }

        public async Task<bool> IsLocationCodeUniqueAsync(string code, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(w => w.LocationCode == code && w.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(w => w.LocationCode == code);
        }
    }
}