using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Doctors;

public record DeleteDoctorCommand(int Id) : IRequest;

public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand>
{
    private readonly IDoctorRepository _repository;
    public DeleteDoctorCommandHandler(IDoctorRepository repository) => _repository = repository;

    public async Task Handle(DeleteDoctorCommand request, CancellationToken ct)
    {
        var doctor = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Лекарят не съществува.");

        await _repository.DeleteAsync(doctor, ct);
        await _repository.SaveChangesAsync(ct);
    }
}