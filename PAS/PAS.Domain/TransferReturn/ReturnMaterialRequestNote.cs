using System;
using Domain.Catalog;
using Domain.Common;
using Domain.PropertyManagement;
using Domain.Receiving;
using Domain.Storage;
namespace Domain.TransferReturn
{
    public class ReturnMaterialRequestNote : BaseEntity
    {
        public Guid ItemId { get; private set; }

        public int Quantity { get; private set; }

        public string Reason { get; private set; }

        public string? ReturnNumber { get; private set; }

        public string? Status { get; private set; }

        public string? ReturnType { get; private set; }

        public Guid? RequestedById { get; private set; }

        public Guid? ApprovedById { get; private set; }

        public Guid? SupplierId { get; private set; }

        public Guid? SourceLocationId { get; private set; }

        public Guid? SourceShelfId { get; private set; }

        public DateTime RequestDate { get; private set; }

        public ItemMaster? Item { get; private set; }

        public Supplier? Supplier { get; private set; }

        public PropertyLocation? SourceLocation { get; private set; }

        public ShelfLocation? SourceShelf { get; private set; }

        private ReturnMaterialRequestNote()
        {
        }

        public ReturnMaterialRequestNote(Guid itemId, int qty, string reason)
        {
            ItemId = itemId;
            Quantity = qty;
            Reason = reason;
            RequestDate = DateTime.UtcNow;
        }
    }
}

