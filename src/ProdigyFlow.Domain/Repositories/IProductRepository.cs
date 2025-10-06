using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Entities.Products;

namespace ProdigyFlow.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}