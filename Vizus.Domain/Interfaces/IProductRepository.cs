using Vizus.Domain.Entities;

namespace Vizus.Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Product product, CancellationToken ct);   // нов ред
    Task SaveChangesAsync(CancellationToken ct);
}