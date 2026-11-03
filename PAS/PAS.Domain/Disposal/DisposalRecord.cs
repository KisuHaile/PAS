using System;
using Domain.Common;
namespace Domain.Disposal
{
    public class DisposalRecord : BaseEntity
    {
        public Guid ItemId { get; private set; }

        public int Quantity { get; private set; }

        public DateTime DisposalDate { get; private set; }

        public Guid DisposedById { get; private set; }

        public string Reason { get; private set; } = string.Empty;

        public string? Status { get; private set; }

        public string? DisposalMethod { get; private set; }

        public decimal? EstimatedValue { get; private set; }

        public Catalog.ItemMaster? Item { get; private set; }

        public DateTime? ApprovedDate { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private DisposalRecord()
        {
        }

        public DisposalRecord(Guid itemId, int quantity, Guid disposedBy, string? reason = null)
        {
            ItemId = itemId;
            Quantity = quantity;
            DisposedById = disposedBy;
            Reason = reason ?? string.Empty;
            DisposalDate = DateTime.UtcNow;
        }

        public void UpdateReason(string reason)
        {
            Reason = reason ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}