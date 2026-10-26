using Domain.Catalog;
using Domain.Common;

namespace Domain.Receiving
{
    public class ReceivingNoteItem : BaseEntity
    {
        public Guid ReceivingNoteId { get; private set; }
        public Guid ItemId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public ReceivingNote? ReceivingNote { get; private set; }
        public ItemMaster? Item { get; private set; }

        private ReceivingNoteItem()
        {
        }

        public ReceivingNoteItem(Guid receivingNoteId, Guid itemId, int quantity, decimal unitPrice)
        {
            ReceivingNoteId = receivingNoteId;
            ItemId = itemId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
