using FluentValidation;

namespace Vizus.Application.Appointments;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .GreaterThan(0);

        RuleFor(x => x.StartTime)
            .Must(startTime => startTime > DateTime.Now)
            .WithMessage("Не можеш да записваш час в миналото.");
        RuleFor(x => x.PatientPhone)
            .NotEmpty().WithMessage("Телефонният номер е задължителен.")
            .Matches(@"^\+?[0-9\s\-]{7,15}$").WithMessage("Невалиден телефонен номер.");
    }
}