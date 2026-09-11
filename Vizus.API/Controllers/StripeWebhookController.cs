using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;

    public StripeWebhookController(IOrderRepository orderRepository, IConfiguration configuration)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(Request.Body).ReadToEndAsync();
        var webhookSecret = _configuration["Stripe:WebhookSecret"];

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                webhookSecret);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;
                var orderId = int.Parse(session!.Metadata["orderId"]);

                var order = await _orderRepository.GetByIdAsync(orderId, CancellationToken.None);
                if (order is not null)
                {
                    order.Status = OrderStatus.Paid;
                    await _orderRepository.SaveChangesAsync(CancellationToken.None);
                }
            }

            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest(e.Message);
        }
    }
}