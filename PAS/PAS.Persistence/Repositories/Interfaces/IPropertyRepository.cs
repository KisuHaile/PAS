using Domain.PropertyManagement;

namespace Persistence.Repositories.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<Property?> GetPropertyWithDetailsAsync(Guid id);
        Task<IEnumerable<Property>> GetPropertiesByLocationAsync(Guid locationId);
        Task<IEnumerable<Property>> GetPropertiesByTypeAsync(Guid typeId);
        Task<IEnumerable<Property>> GetPropertiesBySafetyBoxAsync(Guid safetyBoxId);
        Task<bool> IsTagNumberUniqueAsync(string tagNumber, Guid? excludeId = null);
        Task<int> GetTotalPropertyCountAsync();
        Task<decimal> GetTotalPropertyValueAsync();
    }
}