using MediatR;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Orders;

public class OrderItemAdminDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderAdminDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string DeliveryType { get; set; } = string.Empty;
    public string? EcontOfficeName { get; set; }
    public string? DeliveryAddress { get; set; }
    public List<OrderItemAdminDto> Items { get; set; } = new();
}

public record GetAllOrdersQuery : IRequest<List<OrderAdminDto>>;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, List<OrderAdminDto>>
{
    private readonly IOrderRepository _repository;
    public GetAllOrdersQueryHandler(IOrderRepository repository) => _repository = repository;

    public async Task<List<OrderAdminDto>> Handle(GetAllOrdersQuery request, CancellationToken ct)
    {
        var orders = await _repository.GetAllAsync(ct);

        return orders.Select(o => new OrderAdminDto
        {
            Id = o.Id,
            CreatedAt = o.CreatedAt,
            Status = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            RecipientName = o.RecipientName,
            RecipientPhone = o.RecipientPhone,
            RecipientEmail = o.RecipientEmail,
            DeliveryType = o.DeliveryType,
            EcontOfficeName = o.EcontOfficeName,
            DeliveryAddress = o.DeliveryAddress,
            Items = o.OrderItems.Select(i => new OrderItemAdminDto
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        }).ToList();
    }
}