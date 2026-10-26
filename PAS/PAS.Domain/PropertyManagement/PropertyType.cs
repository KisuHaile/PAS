using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.PropertyManagement
{
    public class PropertyType : BaseEntity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Guid? PropertyCategoryId { get; private set; }

        public PropertyCategory PropertyCategory { get; private set; }

        public ICollection<Property> Properties { get; private set; } = new List<Property>();

        private PropertyType()
        {
        }

        public PropertyType(string name, string description, Guid? propertyCategoryId = null)
        {
            Name = name;
            Description = description;
            PropertyCategoryId = propertyCategoryId;
        }
    }
}