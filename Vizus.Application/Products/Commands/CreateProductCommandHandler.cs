using MediatR;
using Vizus.Domain.Entities;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;

    public CreateProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId
        };

        await _repository.AddAsync(product, ct);
        await _repository.SaveChangesAsync(ct);

        return product.Id;
    }
}