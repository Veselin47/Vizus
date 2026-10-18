using MediatR;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Orders;

public record UpdateOrderStatusCommand(int Id, OrderStatus Status) : IRequest;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _repository;
    public UpdateOrderStatusCommandHandler(IOrderRepository repository) => _repository = repository;

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Поръчката не съществува.");

        order.Status = request.Status;
        await _repository.SaveChangesAsync(ct);
    }
}