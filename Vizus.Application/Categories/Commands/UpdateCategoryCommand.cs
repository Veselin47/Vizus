using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Categories.Commands;

public record UpdateCategoryCommand(int Id, string Name, int? ParentCategoryId) : IRequest;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _repository;
    public UpdateCategoryCommandHandler(ICategoryRepository repository) => _repository = repository;

    public async Task Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Категорията не съществува.");

        category.Name = request.Name;
        category.ParentCategoryId = request.ParentCategoryId;

        await _repository.SaveChangesAsync(ct);
    }
}