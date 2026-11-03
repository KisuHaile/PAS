using System;
using Domain.Common;
using Domain.Catalog;
using Domain.Storage;

namespace Domain.Requisition
{
    public class SR_Detail : BaseEntity
    {
        public Guid SRId { get; private set; }

        public Guid ItemId { get; private set; }

        public Guid? ShelfId { get; private set; }

        public int RequestedQty { get; private set; }

        public int IssuedQty { get; private set; }

        // Navigation properties
        public ItemMaster? Item { get; private set; }

        public ShelfLocation? ShelfLocation { get; private set; }

        public ShelfLocation? Shelf => ShelfLocation;

        private SR_Detail()
        {
        }

        public SR_Detail(Guid srId, Guid itemId, int requested)
        {
            SRId = srId;
            ItemId = itemId;
            RequestedQty = requested;
        }

        public void Issue(int qty)
        {
            IssuedQty = qty;
        }
    }
}