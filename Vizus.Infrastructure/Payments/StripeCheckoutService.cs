using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Entities;

namespace Vizus.Infrastructure.Payments;

public class StripeCheckoutService : IPaymentService
{
    private readonly IConfiguration _configuration;

    public StripeCheckoutService(IConfiguration configuration)
    {
        _configuration = configuration;
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
    }

    public async Task<(string SessionId, string CheckoutUrl)> CreateCheckoutSessionAsync(Order order, CancellationToken ct)
    {
        var lineItems = order.OrderItems.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "eur",
                UnitAmount = (long)(item.UnitPrice * 100), // Stripe работи в стотинки, не в лева
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.ProductName
                }
            },
            Quantity = item.Quantity
        }).ToList();

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = _configuration["Stripe:SuccessUrl"] + "?orderId=" + order.Id,
            CancelUrl = _configuration["Stripe:CancelUrl"] + "?orderId=" + order.Id,
            Metadata = new Dictionary<string, string> { { "orderId", order.Id.ToString() } }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);

        return (session.Id, session.Url);
    }
}