namespace Catalog.Endpoint.Messages.Events;

// Published when an order is finalised by Catalog (alongside
// OrderAccepted). Carries both the fulfilled line items (so projection
// subscribers - notably Marketing's popularity/trending counters -
// have everything they need without reading from Catalog) and the
// items that were ordered but could not be fulfilled, so Finance can
// adjust the charged amount and the email can list them to the
// customer.
public sealed record OrderPlaced
{
    public required Guid OrderId { get; init; }
    public required DateTime OccurredAt { get; init; }
    public required List<OrderedItem> Items { get; init; }
    public required List<UnfulfilledItem> UnfulfilledItems { get; init; }
}

public sealed record OrderedItem
{
    public required Guid ProductId { get; init; }
    public required int FulfilledQuantity { get; init; }
}

public sealed record UnfulfilledItem
{
    public required Guid ProductId { get; init; }
    public required int ShortfallQuantity { get; init; }
}
