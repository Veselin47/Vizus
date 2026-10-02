namespace Vizus.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;   // snapshot - цената/името може да се промени по-късно
    public decimal UnitPrice { get; set; }                     // snapshot
    public int Quantity { get; set; }
    public string? ProductImageUrl { get; set; }
}