using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vizus.Application.Shipping;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly IMediator _mediator;
    public ShippingController(IMediator mediator) => _mediator = mediator;

    [HttpGet("econt/cities")]
    public async Task<IActionResult> GetEcontCities([FromQuery] string? search, CancellationToken ct)
        => Ok(await _mediator.Send(new GetEcontCitiesQuery(search), ct));

    [HttpGet("econt/offices")]
    public async Task<IActionResult> GetEcontOffices([FromQuery] int cityId, CancellationToken ct)
        => Ok(await _mediator.Send(new GetEcontOfficesQuery(cityId), ct));
}