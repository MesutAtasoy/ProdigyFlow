using Moq;
using ProdigyFlow.Application.Services;
using ProdigyFlow.Domain.Entities;
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
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product> { new Product { Name = "Test" , Sku = "abc"} });
        var result = await _service.GetAllProductsAsync();
        Assert.Single(result);
    }
}