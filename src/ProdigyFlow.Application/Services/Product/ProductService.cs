using ProdigyFlow.Domain.Repositories;

namespace ProdigyFlow.Application.Services.Product;

public class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Domain.Entities.Products.Product>> GetAllProductsAsync() => _repository.GetAllAsync();
    public Task<Domain.Entities.Products.Product?> GetProductAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddProductAsync(Domain.Entities.Products.Product product) => _repository.AddAsync(product);
    public Task UpdateProductAsync(Domain.Entities.Products.Product product) => _repository.UpdateAsync(product);
    public Task DeleteProductAsync(Guid id) => _repository.DeleteAsync(id);
}