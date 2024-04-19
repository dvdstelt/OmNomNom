using System.Collections.Concurrent;

namespace Catalog.Data.Models;

public class Order
{
    public Guid OrderId { get; set; }

    public ConcurrentDictionary<Guid, OrderProduct> Products { get; set; } = new();
}

/// <summary>
/// We don't want to copy all the information to the order.
/// Quantity is used to calculate inventory, but Product doesn't have that.
/// That's why we create this entity
/// </summary>
public class OrderProduct
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}