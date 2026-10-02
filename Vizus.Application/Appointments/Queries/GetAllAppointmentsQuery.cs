using MediatR;

namespace Vizus.Application.Appointments;

public record GetAllAppointmentsQuery : IRequest<List<AppointmentAdminDto>>;