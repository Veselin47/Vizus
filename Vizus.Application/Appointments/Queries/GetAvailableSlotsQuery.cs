using MediatR;

namespace Vizus.Application.Appointments;

public record GetAvailableSlotsQuery(int DoctorId, DateOnly Date) : IRequest<List<TimeSlotDto>>;