using MediatR;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public record ConfirmAppointmentCommand(string Token) : IRequest;

public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand>
{
    private readonly IAppointmentRepository _repository;
    public ConfirmAppointmentCommandHandler(IAppointmentRepository repository) => _repository = repository;

    public async Task Handle(ConfirmAppointmentCommand request, CancellationToken ct)
    {
        var appointment = await _repository.GetByConfirmationTokenAsync(request.Token, ct)
            ?? throw new KeyNotFoundException("Невалиден линк за потвърждение.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Този час е отказан и не може да бъде потвърден.");

        // идемпотентно: втори клик върху линка не причинява грешка
        if (appointment.Status == AppointmentStatus.Confirmed)
            return;

        appointment.Status = AppointmentStatus.Confirmed;
        appointment.ConfirmedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync(ct);
    }
}