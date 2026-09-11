using MediatR;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Categories.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryCommandHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var category = new Category
        {
            Name = request.Name,
            ParentCategoryId = request.ParentCategoryId
        };

        await _repository.AddAsync(category, ct);
        await _repository.SaveChangesAsync(ct);

        return category.Id;
    }
}