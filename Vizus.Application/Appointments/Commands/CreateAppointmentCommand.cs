using MediatR;

namespace Vizus.Application.Appointments;

public record CreateAppointmentCommand(string PatientUserId, int DoctorId, DateTime StartTime, string PatientPhone) : IRequest<int>;