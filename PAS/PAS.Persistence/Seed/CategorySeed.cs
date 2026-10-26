using Domain.Catalog;
using Persistence.Context;
using System.Collections.Generic;
using System.Linq;

namespace Persistence.Seed
{
    public static class CategorySeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category("Electronics", "Electronic items"),
                    new Category("Furniture", "Office furniture"),
                    new Category("Stationery", "Office stationery")
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
