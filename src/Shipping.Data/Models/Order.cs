namespace Shipping.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    // Filled in as the checkout flow progresses; the row needs to be
    // valid before the customer reaches the address step.
    public Address? Address { get; set; }
    public Guid? DeliveryOptionId { get; set; }
}