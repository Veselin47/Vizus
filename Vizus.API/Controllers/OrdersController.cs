using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vizus.Application.Orders;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]   // трябва да си логнат, за да пазаруваш - но е достъпно за всеки логнат (не само Admin)
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var command = new CreateOrderCommand(
            userId, request.Items, request.RecipientName, request.RecipientPhone,
            request.RecipientEmail, request.DeliveryType, request.EcontOfficeCode,
            request.EcontOfficeName, request.DeliveryAddress);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    public record CreateOrderRequest(
        List<OrderItemRequest> Items, string RecipientName, string RecipientPhone,
        string RecipientEmail, string DeliveryType, string? EcontOfficeCode,
        string? EcontOfficeName, string? DeliveryAddress);
}