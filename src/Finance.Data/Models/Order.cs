namespace Finance.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public Address BillingAddress { get; set; } = new();
    public Guid DeliveryOptionId { get; set; }
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}