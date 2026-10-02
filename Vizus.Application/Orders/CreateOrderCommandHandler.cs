using MediatR;
using Vizus.Application.Common.Interfaces;
using Vizus.Domain.Entities;
using Vizus.Domain.Enums;
using Vizus.Domain.Interfaces;

namespace Vizus.Application.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;

    public CreateOrderCommandHandler(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IPaymentService paymentService)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _paymentService = paymentService;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        if (request.Items.Count == 0)
            throw new ArgumentException("Количката е празна.");

        var orderItems = new List<OrderItem>();
        decimal total = 0;

        foreach (var item in request.Items)
        {
            // ВАЖНО: никога не вярваме на цена, изпратена от клиента - винаги четем от базата
            var product = await _productRepository.GetByIdAsync(item.ProductId, ct)
                ?? throw new KeyNotFoundException($"Продукт с Id {item.ProductId} не съществува.");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Недостатъчна наличност за '{product.Name}'.");

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity,
                ProductImageUrl = product.ImageUrl
            });

            total += product.Price * item.Quantity;
        }

        var order = new Order
        {
            UserId = request.UserId,
            TotalAmount = total,
            Status = OrderStatus.Pending,
            OrderItems = orderItems
        };

        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);   // за да получим order.Id, преди да викаме Stripe

        var (sessionId, checkoutUrl) = await _paymentService.CreateCheckoutSessionAsync(order, ct);

        order.StripeSessionId = sessionId;
        await _orderRepository.SaveChangesAsync(ct);

        return new CreateOrderResult(order.Id, checkoutUrl);
    }
}