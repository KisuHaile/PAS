using System.Collections.Generic;
using Domain.Common;

namespace Domain.PropertyManagement
{
    public class PropertyLocation : BaseEntity
    {
        public string Name { get; private set; }

        public string LocationType { get; private set; }

        public ICollection<Property> Properties { get; private set; } = new List<Property>();

        public ICollection<SafetyBox> SafetyBoxes { get; private set; } = new List<SafetyBox>();

        private PropertyLocation()
        {
        }

        public PropertyLocation(string name, string type)
        {
            Name = name;
            LocationType = type;
        }
    }
}