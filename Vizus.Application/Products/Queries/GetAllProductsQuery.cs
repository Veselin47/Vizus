using MediatR;

namespace Vizus.Application.Products.Queries;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;