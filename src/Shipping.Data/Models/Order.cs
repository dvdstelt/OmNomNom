namespace Shipping.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid LocationId { get; set; }
    public Guid CustomerId { get; set; }
    public Address Address { get; set; }
    public Guid? DeliveryOptionId { get; set; }
}