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

    // The amount Finance committed to charge for this order. Set by
    // OrderPlacedHandler once the fulfilment outcome is known and the
    // billed amount can be computed from the fulfilled lines plus
    // shipping. Stays at 0 for cancelled orders. The email composer
    // reads this rather than recomputing on the fly so the displayed
    // total matches what was billed.
    public decimal ChargedAmount { get; set; }
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

    // Defaults to true at submit time; flipped to false by
    // OrderPlacedHandler / OrderCancelledHandler for items Catalog
    // could not fulfil. The customer is only charged for items where
    // this is true.
    public bool Fulfilled { get; set; } = true;
}
