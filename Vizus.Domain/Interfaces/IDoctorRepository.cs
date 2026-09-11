using Vizus.Domain.Entities;

namespace Vizus.Domain.Interfaces;

public interface IDoctorRepository
{
    Task<List<Doctor>> GetAllAsync(CancellationToken ct);
    Task<Doctor?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Doctor doctor, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}