using MediatR;

namespace Vizus.Application.Doctors;

public record CreateDoctorCommand(
    string FullName, string Specialty, TimeSpan WorkStartTime, TimeSpan WorkEndTime,
    string WorkingDaysCsv, string? ImageUrl, string? Bio, int? YearsOfExperience) : IRequest<int>;