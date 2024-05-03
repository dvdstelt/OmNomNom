using System.Collections.Concurrent;

namespace Catalog.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public Guid LocationId { get; set; }
    public List<OrderItem> Products { get; set; } = [];
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}