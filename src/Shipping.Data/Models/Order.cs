namespace Shipping.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public Address Address { get; set; } = new();
    public Guid DeliveryOptionId { get; set; }
}