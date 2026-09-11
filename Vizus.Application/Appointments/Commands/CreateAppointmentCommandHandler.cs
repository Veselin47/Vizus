using MediatR;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, int>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMediator _mediator;

    public CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IMediator mediator)
    {
        _appointmentRepository = appointmentRepository;
        _mediator = mediator;
    }

    public async Task<int> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        var date = DateOnly.FromDateTime(request.StartTime);

        // ВАЖНО: препроверяваме наличността на сървъра - никога не вярваме, че frontend списъкът е все още валиден
        // (друг пациент може да е грабнал същия слот междувременно)
        var availableSlots = await _mediator.Send(new GetAvailableSlotsQuery(request.DoctorId, date), ct);

        if (!availableSlots.Any(s => s.Start == request.StartTime))
            throw new InvalidOperationException("Избраният час вече не е свободен.");

        var appointment = new Appointment
        {
            DoctorId = request.DoctorId,
            PatientUserId = request.PatientUserId,
            StartTime = request.StartTime,
            EndTime = request.StartTime.AddMinutes(30)
        };

        await _appointmentRepository.AddAsync(appointment, ct);
        await _appointmentRepository.SaveChangesAsync(ct);

        return appointment.Id;
    }
}