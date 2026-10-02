using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Vizus.Domain.Enums;

namespace Vizus.Application.Appointments.Commands
{
    public record UpdateAppointmentStatusCommand(int Id, AppointmentStatus Status) : IRequest;
}
