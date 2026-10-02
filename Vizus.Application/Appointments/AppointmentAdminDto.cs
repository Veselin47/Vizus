namespace Vizus.Application.Appointments;

public class AppointmentAdminDto
{
    public int Id { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientEmail { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
}