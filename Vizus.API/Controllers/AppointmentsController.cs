using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vizus.Application.Appointments;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AppointmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateOnly date, CancellationToken ct)
        => Ok(await _mediator.Send(new GetAvailableSlotsQuery(doctorId, date), ct));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var id = await _mediator.Send(new CreateAppointmentCommand(userId, request.DoctorId, request.StartTime), ct);
        return Ok(new { id });
    }
}

public record CreateAppointmentRequest(int DoctorId, DateTime StartTime);