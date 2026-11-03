using System;
using System.Collections.Generic;
using Domain.Common;
using Domain.Requisition;
using Domain.Storage;

namespace Domain.Catalog
{
    public class ItemMaster : BaseEntity
    {
        public string SKU { get; private set; }

        public string ItemName { get; private set; }

        public Guid CategoryId { get; private set; }

        public string UnitOfMeasure { get; private set; }

        public bool RequiresInspection { get; private set; }

        public int MinStockLevel { get; private set; }

        public decimal UnitPrice { get; private set; }

        public string Status { get; private set; } = "Active";

        public Category Category { get; private set; }

        public ICollection<InventoryStock> InventoryStocks { get; private set; } = new List<InventoryStock>();

        public ICollection<SR_Detail> ServiceRequestDetails { get; private set; } = new List<SR_Detail>();

        private ItemMaster()
        {
        }

        public ItemMaster(string sku, string itemName, Guid categoryId, string uom, bool inspection, int minStock, decimal unitPrice = 0m, string status = "Active")
        {
            SKU = sku;
            ItemName = itemName;
            CategoryId = categoryId;
            UnitOfMeasure = uom;
            RequiresInspection = inspection;
            MinStockLevel = minStock;
            UnitPrice = unitPrice;
            Status = status;
        }
    }
}