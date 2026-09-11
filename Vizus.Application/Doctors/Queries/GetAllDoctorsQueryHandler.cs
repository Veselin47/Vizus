using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Doctors;

public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, List<DoctorDto>>
{
    private readonly IDoctorRepository _repository;

    public GetAllDoctorsQueryHandler(IDoctorRepository repository) => _repository = repository;

    public async Task<List<DoctorDto>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
    {
        var doctors = await _repository.GetAllAsync(ct);
        return doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            FullName = d.FullName,
            Specialty = d.Specialty
        }).ToList();
    }
}