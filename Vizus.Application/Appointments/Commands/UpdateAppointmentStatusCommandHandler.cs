using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Appointments.Commands
{
    public class UpdateAppointmentStatusCommandHandler : IRequestHandler<UpdateAppointmentStatusCommand>
    {
        private readonly IAppointmentRepository _repository;

        public UpdateAppointmentStatusCommandHandler(IAppointmentRepository repository) => _repository = repository;

        public async Task Handle(UpdateAppointmentStatusCommand request, CancellationToken ct)
        {
            var appointment = await _repository.GetByIdAsync(request.Id, ct)
                ?? throw new KeyNotFoundException("Часът не съществува.");

            appointment.Status = request.Status;
            await _repository.SaveChangesAsync(ct);
        }
    }
}
