using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Repositories;

namespace ProdigyFlow.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Category>> GetAllCategoriesAsync() => _repository.GetAllAsync();
    public Task<Category?> GetCategoryAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddCategoryAsync(Category category) => _repository.AddAsync(category);
    public Task UpdateCategoryAsync(Category category) => _repository.UpdateAsync(category);
    public Task DeleteCategoryAsync(Guid id) => _repository.DeleteAsync(id);
}