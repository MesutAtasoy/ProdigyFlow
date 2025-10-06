namespace ProdigyFlow.Application.Services.Product;

public class ProductCreateRequest
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }

    public string Barcode { get; set; }
}