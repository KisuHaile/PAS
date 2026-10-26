using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.PropertyManagement
{
    public class SafetyBox : BaseEntity
    {
        public string BoxNumber { get; private set; }

        public int TotalShelves { get; private set; }

        public Guid LocationId { get; private set; }

        public PropertyLocation Location { get; private set; }

        public ICollection<Property> Properties { get; private set; } = new List<Property>();

        public ICollection<SafetyBoxShelf> Shelves { get; private set; } = new List<SafetyBoxShelf>();

        private SafetyBox()
        {
        }

        public SafetyBox(string boxNumber, int shelves, Guid locationId)
        {
            BoxNumber = boxNumber;
            TotalShelves = shelves;
            LocationId = locationId;
        }
    }
}
