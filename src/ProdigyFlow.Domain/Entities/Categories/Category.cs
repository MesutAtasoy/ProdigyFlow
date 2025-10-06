using ProdigyFlow.Domain.Entities.Products;

namespace ProdigyFlow.Domain.Entities.Categories;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}