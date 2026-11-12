using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Seed
{
    public static class ItemMasterSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!await context.ItemMasters.AnyAsync())
            {
                var electronics = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
                var furniture = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Furniture");
                var stationery = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Stationery");

                var items = new List<ItemMaster>();

                if (electronics != null)
                {
                    items.Add(new ItemMaster("IT-LAP-001", "Dell Latitude 5540 Laptop 15.6\"", electronics.Id, "PCS", false, 5, 25000m));
                    items.Add(new ItemMaster("IT-MON-002", "HP 27\" Full HD IPS Monitor", electronics.Id, "PCS", false, 5, 8000m));
                    items.Add(new ItemMaster("IT-ACC-005", "Logitech Wireless Keyboard & Mouse Combo", electronics.Id, "SET", false, 10, 1500m));
                    items.Add(new ItemMaster("IT-CAB-006", "Cat6 Gigabit Ethernet Cable (3M)", electronics.Id, "PCS", false, 10, 250m));
                }

                if (furniture != null)
                {
                    items.Add(new ItemMaster("FUR-CHR-003", "Ergonomic High-Back Office Chair", furniture.Id, "PCS", false, 3, 7500m));
                }

                if (stationery != null)
                {
                    items.Add(new ItemMaster("STN-NOT-004", "Hardcover Executive Notebook (A5)", stationery.Id, "PCS", false, 20, 200m));
                }

                if (items.Count > 0)
                {
                    await context.ItemMasters.AddRangeAsync(items);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
