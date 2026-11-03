using Domain.Catalog;
using System;

namespace Persistence.Specifications
{
    public class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification()
        {
            // No includes by default. Domain model exposes SubCategories, not Items.
        }

        public CategorySpecification(Guid id) : base(c => c.Id == id)
        {
            // No includes by default.
        }
    }
}
