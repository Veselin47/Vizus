using MediatR;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, List<TimeSlotDto>>
{
    private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAvailableSlotsQueryHandler(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<List<TimeSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken ct)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId, ct)
            ?? throw new KeyNotFoundException("Лекарят не съществува.");

        // 1. Проверка дали докторът въобще работи в този ден от седмицата
        var dayOfWeek = request.Date.DayOfWeek;
        if (!doctor.GetWorkingDays().Contains(dayOfWeek))
            return new List<TimeSlotDto>();

        // 2. Проверка за отпуск на конкретната дата
        var timeOffs = await _appointmentRepository.GetTimeOffAsync(request.DoctorId, request.Date, ct);
        if (timeOffs.Any())
            return new List<TimeSlotDto>();

        // 3. Генерираме всички теоретични 30-мин слотове в рамките на работния ден
        var allSlots = new List<TimeSlotDto>();
        var current = request.Date.ToDateTime(TimeOnly.FromTimeSpan(doctor.WorkStartTime));
        var end = request.Date.ToDateTime(TimeOnly.FromTimeSpan(doctor.WorkEndTime));

        while (current + SlotDuration <= end)
        {
            allSlots.Add(new TimeSlotDto(current, current + SlotDuration));
            current += SlotDuration;
        }

        // 4. Изваждаме вече заетите слотове (Pending или Confirmed - Cancelled не пречи)
        var existingAppointments = await _appointmentRepository.GetByDoctorAndDateAsync(request.DoctorId, request.Date, ct);
        var occupied = existingAppointments
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.StartTime)
            .ToHashSet();

        return allSlots.Where(slot => !occupied.Contains(slot.Start)).ToList();
    }
}