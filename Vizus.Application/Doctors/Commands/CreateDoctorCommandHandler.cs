using MediatR;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Doctors;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, int>
{
    private readonly IDoctorRepository _repository;

    public CreateDoctorCommandHandler(IDoctorRepository repository) => _repository = repository;

    public async Task<int> Handle(CreateDoctorCommand request, CancellationToken ct)
    {
        var doctor = new Doctor
        {
            FullName = request.FullName,
            Specialty = request.Specialty,
            WorkStartTime = request.WorkStartTime,
            WorkEndTime = request.WorkEndTime,
            Bio = request.Bio,
            YearsOfExperience = request.YearsOfExperience,
            WorkingDaysCsv = request.WorkingDaysCsv
        };

        await _repository.AddAsync(doctor, ct);
        await _repository.SaveChangesAsync(ct);
        return doctor.Id;
    }
}