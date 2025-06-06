using App.Domain.Entities.Common;

namespace App.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }

    // Navigation Property
    public List<Product>? Products { get; set; }
}