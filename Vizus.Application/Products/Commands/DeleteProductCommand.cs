using MediatR;

namespace Vizus.Application.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest;