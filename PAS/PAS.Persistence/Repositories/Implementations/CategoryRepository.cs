using Microsoft.EntityFrameworkCore;
using Domain.Catalog;
using Persistence.Context;
using Persistence.Repositories.Interfaces;

namespace Persistence.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithSubCategoriesAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoryHierarchyAsync()
        {
            var categories = await _dbSet
                .Include(c => c.SubCategories)
                .ToListAsync();

            return categories.Where(c => c.ParentCategoryId == null);
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(c => c.Name == name && c.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(c => c.Name == name);
        }
    }
}