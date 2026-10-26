using System;
using Domain.Catalog;
using Domain.Common;
using Domain.PropertyManagement;
using Domain.Storage;
namespace Domain.TransferReturn
{
    public class TransferRecord : BaseEntity
    {
        public Guid ItemId { get; private set; }

        public Guid FromLocationId { get; private set; }

        public Guid ToLocationId { get; private set; }

        public int Quantity { get; private set; }

        public DateTime TransferDate { get; private set; }

        public string? TransferNumber { get; private set; }

        public Guid? InitiatedById { get; private set; }

        public Guid? ApprovedById { get; private set; }

        public Guid? FromShelfId { get; private set; }

        public Guid? ToShelfId { get; private set; }

        public DateTime? ApprovedDate { get; private set; }

        public string? Status { get; private set; }

        public string? BatchNumber { get; private set; }

        public DateTime? ExpiryDate { get; private set; }

        public string? Reason { get; private set; }

        public string? Remarks { get; private set; }

        public string? Reference { get; private set; }

        public ItemMaster Item { get; private set; }

        public PropertyLocation FromLocation { get; private set; }

        public PropertyLocation ToLocation { get; private set; }

        public ShelfLocation? FromShelf { get; private set; }

        public ShelfLocation? ToShelf { get; private set; }

        private TransferRecord()
        {
        }

        public TransferRecord(Guid itemId, Guid from, Guid to, int qty)
        {
            ItemId = itemId;
            FromLocationId = from;
            ToLocationId = to;
            Quantity = qty;
            TransferDate = DateTime.UtcNow;
        }
    }
}
