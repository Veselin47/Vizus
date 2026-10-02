using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vizus.Application.Appointments;
using Vizus.Application.Appointments.Commands;
using Vizus.Domain.Enums;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AppointmentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllAppointmentsQuery(), ct));

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

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateStatusRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<AppointmentStatus>(request.Status, out var status))
            return BadRequest("Невалиден статус.");

        await _mediator.Send(new UpdateAppointmentStatusCommand(id, status), ct);
        return NoContent();
    }

    public record UpdateStatusRequest(string Status);
    [HttpPost("{id}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await _mediator.Send(new CancelAppointmentCommand(id), ct);
        return NoContent();
    }
}

public record CreateAppointmentRequest(int DoctorId, DateTime StartTime);