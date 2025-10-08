using ProdigyFlow.Domain.Entities.Categories;

namespace ProdigyFlow.Domain.Entities.Products;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public string Barcode { get; set; }
    
    public string Image { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public Category? Category { get; set; }
}