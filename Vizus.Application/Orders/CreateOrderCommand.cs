using MediatR;

namespace Vizus.Application.Orders;

public record OrderItemRequest(int ProductId, int Quantity);

public record CreateOrderCommand(string UserId, List<OrderItemRequest> Items) : IRequest<CreateOrderResult>;

public record CreateOrderResult(int OrderId, string CheckoutUrl);