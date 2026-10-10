using Vizus.Domain.Enums;

namespace Vizus.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? StripeSessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string DeliveryType { get; set; } = string.Empty; // "Pickup", "EcontOffice", "EcontAddress"
    public string? EcontOfficeCode { get; set; }
    public string? EcontOfficeName { get; set; }
    public string? DeliveryAddress { get; set; }


    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}