using Microsoft.EntityFrameworkCore;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;
using Vizus.Infrastructure.Persistence;

namespace Vizus.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Doctor>> GetAllAsync(CancellationToken ct)
        => await _context.Doctors.ToListAsync(ct);

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Doctors.FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task AddAsync(Doctor doctor, CancellationToken ct)
        => await _context.Doctors.AddAsync(doctor, ct);
    public async Task DeleteAsync(Doctor doctor, CancellationToken ct)
    => await Task.Run(() => _context.Doctors.Remove(doctor), ct);
    public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}