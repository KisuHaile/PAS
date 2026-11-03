using Domain.Storage;

namespace Persistence.Repositories.Interfaces
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse>
    {
        Task<Warehouse?> GetWarehouseWithShelvesAsync(Guid id);
        Task<bool> IsWarehouseNameUniqueAsync(string name, Guid? excludeId = null);
        Task<bool> IsLocationCodeUniqueAsync(string code, Guid? excludeId = null);
    }
}