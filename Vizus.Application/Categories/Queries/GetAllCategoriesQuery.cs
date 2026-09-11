using MediatR;

namespace Vizus.Application.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;