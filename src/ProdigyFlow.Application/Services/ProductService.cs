using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Repositories;

namespace ProdigyFlow.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetAllProductsAsync() => _repository.GetAllAsync();
    public Task<Product?> GetProductAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddProductAsync(Product product) => _repository.AddAsync(product);
    public Task UpdateProductAsync(Product product) => _repository.UpdateAsync(product);
    public Task DeleteProductAsync(Guid id) => _repository.DeleteAsync(id);
}