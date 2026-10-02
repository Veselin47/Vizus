using Microsoft.EntityFrameworkCore;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;
using Vizus.Infrastructure.Persistence;

namespace Vizus.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken ct)
        => await _context.Products.Include(p => p.Category).ToListAsync(ct);

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Product product, CancellationToken ct)
        => await _context.Products.AddAsync(product, ct);
    public async Task DeleteAsync(Product product, CancellationToken ct)
    => await Task.Run(() => _context.Products.Remove(product), ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}