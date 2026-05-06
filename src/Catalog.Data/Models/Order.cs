namespace Catalog.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }

    public List<OrderItem> Products { get; set; } = [];
}

public class OrderItem
{
    // Composite PK with ProductId; one row per product per order, since
    // adding the same product again increments Quantity rather than
    // creating a duplicate row.
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}