using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Categories.Commands;

public record DeleteCategoryCommand(int Id) : IRequest;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _repository;
    public DeleteCategoryCommandHandler(ICategoryRepository repository) => _repository = repository;

    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Категорията не съществува.");

        await _repository.DeleteAsync(category, ct);
        await _repository.SaveChangesAsync(ct);
    }
}