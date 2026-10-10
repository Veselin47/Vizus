using MediatR;

namespace Vizus.Application.Orders;

public record OrderItemRequest(int ProductId, int Quantity);

public record CreateOrderCommand(
    string UserId,
    List<OrderItemRequest> Items,
    string RecipientName,
    string RecipientPhone,
    string RecipientEmail,
    string DeliveryType,
    string? EcontOfficeCode,
    string? EcontOfficeName,
    string? DeliveryAddress) : IRequest<CreateOrderResult>;
public record CreateOrderResult(int OrderId, string CheckoutUrl);