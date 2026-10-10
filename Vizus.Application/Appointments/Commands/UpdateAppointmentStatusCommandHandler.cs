using MediatR;
using Microsoft.Extensions.Logging;
using Vizus.Application.Appointments.Commands;
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand>
{
    private readonly IAppointmentRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IUserLookupService _userLookupService;
    private readonly ILogger<UpdateAppointmentStatusCommandHandler> _logger;

    public UpdateAppointmentStatusCommandHandler(
        IAppointmentRepository repository,
        IEmailService emailService,
        IUserLookupService userLookupService,
        ILogger<UpdateAppointmentStatusCommandHandler> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _userLookupService = userLookupService;
        _logger = logger;
    }

    public async Task Handle(UpdateAppointmentStatusCommand request, CancellationToken ct)
    {
        var appointment = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Часът не съществува.");

        // нищо за правене, ако статусът не се променя (и не пращаме втори имейл)
        if (appointment.Status == request.Status)
            return;

        appointment.Status = request.Status;
        await _repository.SaveChangesAsync(ct);

        if (request.Status == AppointmentStatus.Cancelled)
        {
            try
            {
                var email = await _userLookupService.GetEmailByIdAsync(appointment.PatientUserId, ct);
                if (email is not null)
                {
                    await _emailService.SendAppointmentCancelledAsync(
                        email, appointment.Doctor.FullName, appointment.StartTime, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send cancellation email for appointment {AppointmentId}", appointment.Id);
            }
        }
    }
}