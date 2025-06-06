using App.Domain.Entities.Common;

namespace App.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Navigation Property
        public int CategoryId { get; set; } // Foreign Key
        public Category Category { get; set; } = null!;
    }
}