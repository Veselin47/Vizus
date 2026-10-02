using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Products.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommandHandler(IProductRepository repository) => _repository = repository;

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Продуктът не съществува.");

        await _repository.DeleteAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
    }
}