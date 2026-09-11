using MediatR;

namespace Vizus.Application.Doctors;

public record GetAllDoctorsQuery : IRequest<List<DoctorDto>>;