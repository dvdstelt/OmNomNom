namespace Finance.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    // Both BillingAddress and DeliveryOptionId are populated as the
    // checkout flow progresses; an Order created by SubmitOrderItems
    // before the address or delivery has been picked must be valid in
    // its half-formed state.
    public Address? BillingAddress { get; set; }
    public Guid? DeliveryOptionId { get; set; }
}

public class OrderItem : IPriced
{
    // Composite PK (OrderId, ProductId); EF Core sets OrderId from the
    // parent Order's key when items are added through the navigation.
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
}
