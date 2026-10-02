using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vizus.Application.Doctors;

namespace Vizus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DoctorsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _mediator.Send(new GetAllDoctorsQuery(), ct));
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateDoctorRequest request, CancellationToken ct)
    {
        var command = new CreateDoctorCommand(
            request.FullName,
            request.Specialty,
            TimeSpan.Parse(request.WorkStartTime),
            TimeSpan.Parse(request.WorkEndTime),
            request.WorkingDaysCsv,
            request.ImageUrl,
            request.Bio,
            request.YearsOfExperience);

        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateDoctorRequest request, CancellationToken ct)
    {
        await _mediator.Send(new UpdateDoctorCommand(
            id, request.FullName, request.Specialty,
            TimeSpan.Parse(request.WorkStartTime), TimeSpan.Parse(request.WorkEndTime),
            request.WorkingDaysCsv, request.ImageUrl, request.Bio, request.YearsOfExperience), ct);
        return NoContent();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteDoctorCommand(id), ct);
        return NoContent();
    }

    public record CreateDoctorRequest(string FullName, string Specialty, string WorkStartTime, string WorkEndTime, string WorkingDaysCsv, string? ImageUrl, string? Bio, int? YearsOfExperience);
    public record UpdateDoctorRequest(string FullName, string Specialty, string WorkStartTime, string WorkEndTime, string WorkingDaysCsv, string? ImageUrl, string? Bio, int? YearsOfExperience);
}