using MediatR;

namespace Vizus.Application.Products.Commands;

public record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    int CategoryId,
    string? ImageUrl) : IRequest;