using Vizus.Domain.Entities;

namespace Vizus.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAppointmentConfirmationRequestAsync(string toEmail, string doctorName, DateTime startTime, string confirmationToken, CancellationToken ct);
    Task SendOrderConfirmationAsync(Order order, CancellationToken ct);
    Task SendAppointmentCancelledAsync(string toEmail, string doctorName, DateTime startTime, CancellationToken ct);
}