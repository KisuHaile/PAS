using Microsoft.EntityFrameworkCore;
using Domain.PropertyManagement;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Property?> GetPropertyWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .Include(p => p.SafetyBox)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLocationAsync(Guid locationId)
        {
            return await _dbSet
                .Where(p => p.LocationId == locationId)
                .Include(p => p.PropertyType)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByTypeAsync(Guid typeId)
        {
            return await _dbSet
                .Where(p => p.PropertyTypeId == typeId)
                .Include(p => p.Location)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesBySafetyBoxAsync(Guid safetyBoxId)
        {
            return await _dbSet
                .Where(p => p.SafetyBoxId == safetyBoxId)
                .Include(p => p.PropertyType)
                .Include(p => p.Location)
                .ToListAsync();
        }

        public async Task<bool> IsTagNumberUniqueAsync(string tagNumber, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(p => p.TagNumber == tagNumber && p.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(p => p.TagNumber == tagNumber);
        }

        public async Task<int> GetTotalPropertyCountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<decimal> GetTotalPropertyValueAsync()
        {
            return await _dbSet.SumAsync(p => p.UnitPrice * p.Quantity);
        }
    }
}