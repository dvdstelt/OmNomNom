namespace Catalog.Endpoint.Messages.Events;

// Published when an order is finalised by Catalog (alongside
// OrderAccepted). Carries the fulfilled line items so projection
// subscribers - notably Marketing's popularity/trending counters -
// have everything they need without reading from Catalog.
public sealed record OrderPlaced
{
    public required Guid OrderId { get; init; }
    public required DateTime OccurredAt { get; init; }
    public required List<OrderedItem> Items { get; init; }
}

public sealed record OrderedItem
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
