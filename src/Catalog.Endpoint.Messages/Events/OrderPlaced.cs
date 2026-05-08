namespace Catalog.Endpoint.Messages.Events;

// Published when an order is finalised by Catalog (alongside
// OrderAccepted). Carries the fulfilled line items so projection
// subscribers - notably Marketing's popularity/trending counters -
// have everything they need without reading from Catalog.
public class OrderPlaced
{
    public Guid OrderId { get; set; }
    public DateTime OccurredAt { get; set; }
    public List<OrderedItem> Items { get; set; } = [];
}

public class OrderedItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
