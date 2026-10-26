using System;
using Domain.Common;
using Domain.Storage;
namespace Domain.Receiving
{
    public class ReceivingNote : BaseEntity
    {
        public string GRNNumber { get; private set; }

        public Guid SupplierId { get; private set; }

        public string? PONumber { get; private set; }

        public string? InvoiceNumber { get; private set; }

        public DateTime? InvoiceDate { get; private set; }

        public string? DeliveryNoteNumber { get; private set; }

        public string? VehicleNumber { get; private set; }

        public string? DriverName { get; private set; }

        public string? Remarks { get; private set; }

        public DateTime ReceivedDate { get; private set; }

        public string Status { get; private set; }

        public Guid ReceivedById { get; private set; }

        public Supplier? Supplier { get; private set; }

        public InspectionLog? InspectionLog { get; private set; }

        public ICollection<ReceivingNoteItem> Items { get; private set; } = new List<ReceivingNoteItem>();

        public ICollection<StockLedger> StockLedgers { get; private set; } = new List<StockLedger>();

        public ICollection<DocumentAttachment> Attachments { get; private set; } = new List<DocumentAttachment>();

        private ReceivingNote()
        {
        }

        public ReceivingNote(string grn, Guid supplierId, Guid receiver)
        {
            GRNNumber = grn;
            SupplierId = supplierId;
            ReceivedById = receiver;
            ReceivedDate = DateTime.UtcNow;
            Status = "PendingInspection";
        }
    }
}