using Microsoft.EntityFrameworkCore;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;
using Vizus.Infrastructure.Persistence;

namespace Vizus.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentRepository(ApplicationDbContext context) => _context = context;

    public async Task<List<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateOnly date, CancellationToken ct)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = start.AddDays(1);

        return await _context.Appointments
            .Where(a => a.DoctorId == doctorId && a.StartTime >= start && a.StartTime < end)
            .ToListAsync(ct);
    }

    public async Task<List<DoctorTimeOff>> GetTimeOffAsync(int doctorId, DateOnly date, CancellationToken ct)
        => await _context.DoctorTimeOffs
            .Where(t => t.DoctorId == doctorId && t.Date == date)
            .ToListAsync(ct);

    public async Task AddAsync(Appointment appointment, CancellationToken ct)
        => await _context.Appointments.AddAsync(appointment, ct);

    public async Task<Appointment?> GetByIdAsync(int id, CancellationToken ct)
        => await _context.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<List<Appointment>> GetAllAsync(CancellationToken ct)
        => await _context.Appointments
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.StartTime)
            .ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
}