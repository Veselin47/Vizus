using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<StripeWebhookController> _logger;

    public StripeWebhookController(
        IOrderRepository orderRepository,
        IConfiguration configuration,
        IEmailService emailService,
        IProductRepository productRepository,
        ILogger<StripeWebhookController> logger)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;
        _emailService = emailService;
        _productRepository = productRepository;
        _logger = logger;
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

                // Обработваме само неплатена поръчка. Stripe може да прати същото събитие повторно,
                // а поръчката може междувременно да е вече изпратена или завършена.
                if (order is not null && order.Status == OrderStatus.Pending)
                {
                    order.Status = OrderStatus.Paid;

                    // Намаляваме наличността. Всички репозиторита споделят един DbContext,
                    // така че едно SaveChangesAsync записва и статуса, и наличностите.
                    foreach (var item in order.OrderItems)
                    {
                        var product = await _productRepository.GetByIdAsync(item.ProductId, CancellationToken.None);
                        if (product is not null)
                            product.StockQuantity = Math.Max(0, product.StockQuantity - item.Quantity);
                    }

                    await _orderRepository.SaveChangesAsync(CancellationToken.None);

                    try
                    {
                        await _emailService.SendOrderConfirmationAsync(order, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send order confirmation email for order {OrderId}", order.Id);
                    }
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