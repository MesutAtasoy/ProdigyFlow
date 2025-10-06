using Microsoft.EntityFrameworkCore;
using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Repositories;

namespace ProdigyFlow.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<Category>> GetAllAsync()
    {
        return Task.FromResult(_context.Categories
            .Include(c => c.Products) // Include related products
            .AsEnumerable());
    }

    public Task<Category?> GetByIdAsync(Guid id)
    {
        return _context.Categories
            .Include(c => c.Products) // Include related products
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
}