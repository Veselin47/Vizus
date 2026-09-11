using Vizus.Domain.Entities;

namespace Vizus.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(Order order, CancellationToken ct);
}