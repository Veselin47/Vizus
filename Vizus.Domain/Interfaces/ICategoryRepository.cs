using Vizus.Domain.Entities;

namespace Vizus.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken ct);
    Task<Category?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Category category, CancellationToken ct);
    Task DeleteAsync(Category category, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}