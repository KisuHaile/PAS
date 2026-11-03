using Domain.PropertyManagement;
using System;

namespace Persistence.Specifications
{
    public class PropertySpecification : BaseSpecification<Property>
    {
        public PropertySpecification()
        {
            // No includes by default. Domain.Property does not expose Category/Location navigations.
        }

        public PropertySpecification(Guid id) : base(p => p.Id == id)
        {
            // No includes by default.
        }
    }
}
