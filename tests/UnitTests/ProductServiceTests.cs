using Moq;
using ProdigyFlow.Application.Services;
using ProdigyFlow.Application.Services.Product;
using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Entities.Products;
using ProdigyFlow.Domain.Repositories;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Name = "Test Product", Sku = "ABC123", Barcode = "123"}
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Product", result.First().Name);
    }

    [Fact]
    public async Task GetProduct_ReturnsProduct_WhenExists()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "Laptop", Sku = "LAP123" ,Barcode = "123"};
        _mockRepo.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

        // Act
        var result = await _service.GetProductAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Laptop", result!.Name);
    }

    [Fact]
    public async Task GetProduct_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddProduct_CallsRepository()
    {
        // Arrange
        var product = new Product { Name = "Tablet", Sku = "TAB123" };

        // Act
        await _service.AddProductAsync(product);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_CallsRepository()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "Monitor", Sku = "MON123" };

        // Act
        await _service.UpdateProductAsync(product);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(product), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_CallsRepository()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await _service.DeleteProductAsync(id);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}