using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Doctors;

public record UpdateDoctorCommand(
    int Id, string FullName, string Specialty, TimeSpan WorkStartTime, TimeSpan WorkEndTime,
    string WorkingDaysCsv, string? ImageUrl, string? Bio, int? YearsOfExperience) : IRequest;

public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand>
{
    private readonly IDoctorRepository _repository;
    public UpdateDoctorCommandHandler(IDoctorRepository repository) => _repository = repository;

    public async Task Handle(UpdateDoctorCommand request, CancellationToken ct)
    {
        var doctor = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Лекарят не съществува.");

        doctor.FullName = request.FullName;
        doctor.ImageUrl = request.ImageUrl;
        doctor.Specialty = request.Specialty;
        doctor.WorkStartTime = request.WorkStartTime;
        doctor.WorkEndTime = request.WorkEndTime;
        doctor.Bio = request.Bio;
        doctor.YearsOfExperience = request.YearsOfExperience;
        doctor.WorkingDaysCsv = request.WorkingDaysCsv;

        await _repository.SaveChangesAsync(ct);
    }
}