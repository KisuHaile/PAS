using System;
using Domain.Common;
using Domain.Catalog;

namespace Domain.Storage
{
    public class InventoryStock : BaseEntity
    {
        public Guid ItemId { get; private set; }

        public Guid ShelfId { get; private set; }

        public int CurrentQuantity { get; private set; }

        public int ReservedQuantity { get; private set; }

        public string? BatchNumber { get; private set; }

        public DateTime? ExpiryDate { get; private set; }

        private InventoryStock()
        {
        }

        public InventoryStock(Guid itemId, Guid shelfId, int quantity)
        {
            ItemId = itemId;
            ShelfId = shelfId;
            CurrentQuantity = quantity;
        }

        // Navigation properties
        public ItemMaster? Item { get; private set; }

        public ShelfLocation? ShelfLocation { get; private set; }

        public ShelfLocation? Shelf => ShelfLocation;

        public ICollection<StockLedger> StockLedgerEntries { get; private set; } = new List<StockLedger>();

        public bool CheckAvailability(int qty)
        {
            return (CurrentQuantity - ReservedQuantity) >= qty;
        }
    }
}