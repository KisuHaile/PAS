using Domain.Common;

namespace Domain.Storage
{
    public class Warehouse : BaseEntity
    {
        public string WarehouseName { get; private set; }

        public string LocationCode { get; private set; }

        public string? City { get; private set; }

        public bool IsActive { get; private set; } = true;

        private Warehouse()
        {
        }

        public Warehouse(string name, string code)
        {
            WarehouseName = name;
            LocationCode = code;
        }

        // Navigation
        public ICollection<ShelfLocation> Shelves { get; private set; } = new List<ShelfLocation>();

        public ICollection<ShelfLocation> ShelfLocations { get; private set; } = new List<ShelfLocation>();
    }
}