using Domain.Catalog;
using Domain.Receiving;
using Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence.Seed
{
    public static class BusinessDataSeed
    {
        public static readonly Guid CategoryElectronics = new Guid("11111111-0000-0000-0000-000000000001");
        public static readonly Guid CategoryFurniture   = new Guid("11111111-0000-0000-0000-000000000002");
        public static readonly Guid CategoryStationery  = new Guid("11111111-0000-0000-0000-000000000003");

        public static readonly Guid WarehouseMain = new Guid("22222222-0000-0000-0000-000000000001");
        public static readonly Guid WarehouseIT   = new Guid("22222222-0000-0000-0000-000000000002");

        public static readonly Guid ShelfA1 = new Guid("33333333-0000-0000-0000-000000000001");
        public static readonly Guid ShelfA2 = new Guid("33333333-0000-0000-0000-000000000002");
        public static readonly Guid ShelfB1 = new Guid("33333333-0000-0000-0000-000000000003");
        public static readonly Guid ShelfC1 = new Guid("33333333-0000-0000-0000-000000000004");

        public static readonly Guid ItemLaptop   = new Guid("44444444-0000-0000-0000-000000000001");
        public static readonly Guid ItemChair    = new Guid("44444444-0000-0000-0000-000000000002");
        public static readonly Guid ItemPrinter  = new Guid("44444444-0000-0000-0000-000000000003");
        public static readonly Guid ItemLamp     = new Guid("44444444-0000-0000-0000-000000000004");
        public static readonly Guid ItemMonitor  = new Guid("44444444-0000-0000-0000-000000000005");
        public static readonly Guid ItemKeyboard = new Guid("44444444-0000-0000-0000-000000000006");
        public static readonly Guid ItemMouse    = new Guid("44444444-0000-0000-0000-000000000007");
        public static readonly Guid ItemCabinet  = new Guid("44444444-0000-0000-0000-000000000008");
        public static readonly Guid ItemMarker   = new Guid("44444444-0000-0000-0000-000000000009");
        public static readonly Guid ItemPaper    = new Guid("44444444-0000-0000-0000-000000000010");
        public static readonly Guid ItemCable    = new Guid("44444444-0000-0000-0000-000000000011");

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // 1. Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category(CategoryElectronics, "Electronics", "Electronic items and IT hardware"),
                    new Category(CategoryFurniture, "Furniture", "Office and warehouse furniture"),
                    new Category(CategoryStationery, "Stationery", "Office supplies and stationery")
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 2. Warehouses
            if (!await context.Warehouses.AnyAsync())
            {
                var warehouses = new List<Warehouse>
                {
                    new Warehouse(WarehouseMain, "Main Warehouse", "MW-01"),
                    new Warehouse(WarehouseIT, "Central IT Store", "IT-01")
                };
                await context.Warehouses.AddRangeAsync(warehouses);
                await context.SaveChangesAsync();
            }

            // 3. Shelf Locations
            if (!await context.ShelfLocations.AnyAsync())
            {
                var mainWh = await context.Warehouses.FirstOrDefaultAsync(w => w.Id == WarehouseMain)
                             ?? await context.Warehouses.FirstAsync();

                var shelves = new List<ShelfLocation>
                {
                    new ShelfLocation(ShelfA1, mainWh.Id, "A1", "R1", "001"),
                    new ShelfLocation(ShelfA2, mainWh.Id, "A1", "R1", "002"),
                    new ShelfLocation(ShelfB1, mainWh.Id, "B1", "R1", "001"),
                    new ShelfLocation(ShelfC1, mainWh.Id, "C1", "R1", "001")
                };
                await context.ShelfLocations.AddRangeAsync(shelves);
                await context.SaveChangesAsync();
            }

            // 4. Suppliers
            if (!await context.Suppliers.AnyAsync())
            {
                var suppliers = new List<Supplier>
                {
                    new Supplier("Dell Technologies PLC", "John Dell", "TIN-DELL-001"),
                    new Supplier("HP Enterprise Ltd", "Sarah HP", "TIN-HP-002"),
                    new Supplier("Office Modern Furniture", "Abebe K.", "TIN-FURN-003")
                };
                await context.Suppliers.AddRangeAsync(suppliers);
                await context.SaveChangesAsync();
            }

            // 5. Items (ItemMasters)
            if (!await context.ItemMasters.AnyAsync())
            {
                var elect = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics")
                            ?? await context.Categories.FirstAsync();
                var furn = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Furniture")
                           ?? await context.Categories.FirstAsync();
                var stat = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Stationery")
                           ?? await context.Categories.FirstAsync();

                var items = new List<ItemMaster>
                {
                    new ItemMaster(ItemLaptop, "LAP-001", "Laptop Dell XPS 13", elect.Id, "PCS", false, 5, 45000m),
                    new ItemMaster(ItemChair, "CHR-002", "Office Chair Ergonomic", furn.Id, "PCS", false, 3, 8500m),
                    new ItemMaster(ItemPrinter, "PRT-003", "HP Printer LaserJet", elect.Id, "PCS", false, 2, 12000m),
                    new ItemMaster(ItemLamp, "LMP-004", "Desk Lamp LED", stat.Id, "PCS", false, 10, 1200m),
                    new ItemMaster(ItemMonitor, "MON-005", "Monitor 24 inch", elect.Id, "PCS", false, 5, 6500m),
                    new ItemMaster(ItemKeyboard, "KEY-006", "Keyboard Wireless", elect.Id, "PCS", false, 10, 1500m),
                    new ItemMaster(ItemMouse, "MOU-007", "Mouse Optical", elect.Id, "PCS", false, 15, 500m),
                    new ItemMaster(ItemCabinet, "CAB-008", "Filing Cabinet", furn.Id, "PCS", false, 2, 15000m),
                    new ItemMaster(ItemMarker, "MRK-009", "Whiteboard Marker", stat.Id, "PCS", false, 25, 80m),
                    new ItemMaster(ItemPaper, "PAP-010", "Paper A4 Ream", stat.Id, "PCS", false, 50, 450m),
                    new ItemMaster(ItemCable, "IT-CAB-006", "Cat6 Gigabit Ethernet Cable (3M)", elect.Id, "PCS", false, 10, 250m)
                };
                await context.ItemMasters.AddRangeAsync(items);
                await context.SaveChangesAsync();
            }

            // 6. Inventory Stocks
            if (!await context.InventoryStocks.AnyAsync())
            {
                var shelf = await context.ShelfLocations.FirstOrDefaultAsync()
                            ?? await context.ShelfLocations.FirstAsync();
                var items = await context.ItemMasters.ToListAsync();

                var stocks = new List<InventoryStock>();
                foreach (var it in items)
                {
                    stocks.Add(new InventoryStock(it.Id, shelf.Id, 25));
                }
                await context.InventoryStocks.AddRangeAsync(stocks);
                await context.SaveChangesAsync();
            }
        }
    }
}
