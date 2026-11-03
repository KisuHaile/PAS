using Domain.Common;

namespace Domain.Receiving
{
    public class Supplier : BaseEntity
    {
        public string SupplierName { get; private set; }

        public string ContactPerson { get; private set; }

        public string TinNumber { get; private set; }

        public string? Email { get; private set; }

        public ICollection<ReceivingNote> ReceivingNotes { get; private set; } = new List<ReceivingNote>();

        private Supplier()
        {
        }

        public Supplier(string name, string contact, string tin)
        {
            SupplierName = name;
            ContactPerson = contact;
            TinNumber = tin;
        }
    }
}