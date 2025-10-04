using Microsoft.EntityFrameworkCore;
using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Repositories;

namespace ProdigyFlow.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult(_context.Products.AsEnumerable());

    public Task<Product?> GetByIdAsync(Guid id) => _context.Products.FirstOrDefaultAsync(p => p.Id == id);

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}