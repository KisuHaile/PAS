using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.PropertyManagement
{
    public class SafetyBoxShelf : BaseEntity
    {
        public Guid SafetyBoxId { get; private set; }

        public int ShelfNumber { get; private set; }

        public ICollection<Property> Properties { get; private set; } = new List<Property>();

        private SafetyBoxShelf()
        {
        }

        public SafetyBoxShelf(Guid boxId, int shelf)
        {
            SafetyBoxId = boxId;
            ShelfNumber = shelf;
        }
    }
}