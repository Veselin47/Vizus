using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _repository;

    public UpdateProductCommandHandler(IProductRepository repository) => _repository = repository;

    public async Task Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Продуктът не съществува.");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.ImageUrl = request.ImageUrl;

        await _repository.SaveChangesAsync(ct);
    }
}