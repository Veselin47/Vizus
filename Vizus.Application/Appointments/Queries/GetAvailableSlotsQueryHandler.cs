using MediatR;
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public class GetAllAppointmentsQueryHandler : IRequestHandler<GetAllAppointmentsQuery, List<AppointmentAdminDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUserLookupService _userLookupService;

    public GetAllAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IUserLookupService userLookupService)
    {
        _appointmentRepository = appointmentRepository;
        _userLookupService = userLookupService;
    }

    public async Task<List<AppointmentAdminDto>> Handle(GetAllAppointmentsQuery request, CancellationToken ct)
    {
        var appointments = await _appointmentRepository.GetAllAsync(ct);

        var userIds = appointments.Select(a => a.PatientUserId).Distinct().ToList();
        var emails = await _userLookupService.GetEmailsByIdsAsync(userIds, ct);

        return appointments.Select(a => new AppointmentAdminDto
        {
            Id = a.Id,
            DoctorName = a.Doctor.FullName,
            PatientEmail = emails.GetValueOrDefault(a.PatientUserId, "—"),
            StartTime = a.StartTime,
            Status = a.Status.ToString()
        }).ToList();
    }
}