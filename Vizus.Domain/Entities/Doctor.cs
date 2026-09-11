namespace Vizus.Domain.Entities;

public class Doctor
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;

    public TimeSpan WorkStartTime { get; set; } = new TimeSpan(9, 0, 0);
    public TimeSpan WorkEndTime { get; set; } = new TimeSpan(17, 0, 0);

    // CSV на DayOfWeek стойности (0=Неделя, 1=Понеделник ... 6=Събота), напр. "1,2,3,4,5" = Пон-Пет
    public string WorkingDaysCsv { get; set; } = "1,2,3,4,5";

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<DoctorTimeOff> TimeOffs { get; set; } = new List<DoctorTimeOff>();

    public List<DayOfWeek> GetWorkingDays()
        => WorkingDaysCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => (DayOfWeek)int.Parse(d))
            .ToList();
}