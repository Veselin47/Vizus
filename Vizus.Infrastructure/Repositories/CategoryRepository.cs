using Microsoft.EntityFrameworkCore;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;
using Vizus.Infrastructure.Persistence;

namespace Vizus.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync(CancellationToken ct)
        => await _context.Categories.ToListAsync(ct);

    public async Task<Category?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Category category, CancellationToken ct)
        => await _context.Categories.AddAsync(category, ct);
    public async Task DeleteAsync(Category category, CancellationToken ct)
    => await Task.Run(() => _context.Categories.Remove(category), ct);
    public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}