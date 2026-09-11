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
    public async Task<IActionResult> Create(List<OrderItemRequest> items, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _mediator.Send(new CreateOrderCommand(userId, items), ct);
        return Ok(result);
    }
}