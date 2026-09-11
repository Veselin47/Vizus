using MediatR;

namespace Vizus.Application.Categories.Commands;

public record CreateCategoryCommand(string Name, int? ParentCategoryId) : IRequest<int>;