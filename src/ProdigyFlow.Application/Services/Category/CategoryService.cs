using ProdigyFlow.Domain.Repositories;

namespace ProdigyFlow.Application.Services.Category;

public class CategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Domain.Entities.Categories.Category>> GetAllCategoriesAsync() => _repository.GetAllAsync();
    public Task<Domain.Entities.Categories.Category?> GetCategoryAsync(Guid id) => _repository.GetByIdAsync(id);
    public Task AddCategoryAsync(Domain.Entities.Categories.Category category) => _repository.AddAsync(category);
    public Task UpdateCategoryAsync(Domain.Entities.Categories.Category category) => _repository.UpdateAsync(category);
    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category is not null)
            _repository.DeleteAsync(id);
    }
}