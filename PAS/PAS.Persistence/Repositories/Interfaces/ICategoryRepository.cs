using Domain.Catalog;

namespace Persistence.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetCategoryWithSubCategoriesAsync(Guid id);
        Task<IEnumerable<Category>> GetRootCategoriesAsync();
        Task<IEnumerable<Category>> GetCategoryHierarchyAsync();
        Task<bool> IsCategoryNameUniqueAsync(string name, Guid? excludeId = null);
    }
}