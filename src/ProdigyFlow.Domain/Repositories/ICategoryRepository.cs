using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Entities.Categories;

namespace ProdigyFlow.Domain.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
}