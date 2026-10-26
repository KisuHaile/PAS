using Domain.Catalog;
using System;

namespace Persistence.Specifications
{
    public class ItemMasterSpecification : BaseSpecification<ItemMaster>
    {
        public ItemMasterSpecification()
        {
            // No navigation property includes by default.
        }

        public ItemMasterSpecification(Guid categoryId)
            : base(i => i.CategoryId == categoryId)
        {
            ApplyOrderBy(i => i.ItemName);
        }
    }
}
