namespace Vizus.Domain.Entities;

public class DoctorTimeOff
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public DateOnly Date { get; set; }   // цял неработен ден (отпуск)
    public string? Reason { get; set; }
}