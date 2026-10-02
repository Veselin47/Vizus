namespace Vizus.Application.Doctors;

public class DoctorDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string WorkStartTime { get; set; } = string.Empty;
    public string WorkEndTime { get; set; } = string.Empty;
    public string WorkingDaysCsv { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Bio { get; set; }
    public int? YearsOfExperience { get; set; }
}