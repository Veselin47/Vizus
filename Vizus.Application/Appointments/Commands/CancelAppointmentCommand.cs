using MediatR;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments;

public record CancelAppointmentCommand(int Id) : IRequest;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand>
{
    private readonly IAppointmentRepository _repository;
    public CancelAppointmentCommandHandler(IAppointmentRepository repository) => _repository = repository;

    public async Task Handle(CancelAppointmentCommand request, CancellationToken ct)
    {
        var appointment = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Часът не съществува.");

        appointment.Status = AppointmentStatus.Cancelled;
        await _repository.SaveChangesAsync(ct);
    }
}