using MediatR;
using Microsoft.Extensions.Logging; 
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Entities;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, int>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;
    private readonly IUserLookupService _userLookupService;
    private readonly IDoctorRepository _doctorRepository;

    private readonly ILogger<CreateAppointmentCommandHandler> _logger;

    public CreateAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IMediator mediator,
        IEmailService emailService,
        IUserLookupService userLookupService,
        IDoctorRepository doctorRepository,
        ILogger<CreateAppointmentCommandHandler> logger)
    {
        _appointmentRepository = appointmentRepository;
        _mediator = mediator;
        _emailService = emailService;
        _userLookupService = userLookupService;
        _doctorRepository = doctorRepository;
        _logger = logger;
    } 

    public async Task<int> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        var date = DateOnly.FromDateTime(request.StartTime);

        // ВАЖНО: препроверяваме наличността на сървъра 
        var availableSlots = await _mediator.Send(new GetAvailableSlotsQuery(request.DoctorId, date), ct);

        if (!availableSlots.Any(s => s.Start == request.StartTime))
            throw new InvalidOperationException("Избраният час вече не е свободен.");

        var appointment = new Appointment
        {
            DoctorId = request.DoctorId,
            PatientUserId = request.PatientUserId,
            StartTime = request.StartTime,
            EndTime = request.StartTime.AddMinutes(30),
            PatientPhone = request.PatientPhone, 
            ConfirmationToken = Guid.NewGuid().ToString("N"), 
            Status = AppointmentStatus.Pending 
        };

        await _appointmentRepository.AddAsync(appointment, ct);
        await _appointmentRepository.SaveChangesAsync(ct);

        try
        {
            var patientEmail = await _userLookupService.GetEmailByIdAsync(request.PatientUserId, ct);
            var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId, ct);

            if (patientEmail is not null && doctor is not null)
            {
                await _emailService.SendAppointmentConfirmationRequestAsync(
                    patientEmail, doctor.FullName, request.StartTime, appointment.ConfirmationToken!, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send appointment confirmation email for appointment {AppointmentId}", appointment.Id);
        }

        return appointment.Id;
    }
}