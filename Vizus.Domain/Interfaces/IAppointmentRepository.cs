using Vizus.Domain.Entities;

namespace Vizus.Domain.Interfaces;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken ct);
    Task<List<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateOnly date, CancellationToken ct);
    Task<List<DoctorTimeOff>> GetTimeOffAsync(int doctorId, DateOnly date, CancellationToken ct);
    Task AddAsync(Appointment appointment, CancellationToken ct);
    Task<List<Appointment>> GetAllAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}