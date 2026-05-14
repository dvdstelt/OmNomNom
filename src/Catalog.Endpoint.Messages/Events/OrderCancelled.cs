namespace Catalog.Endpoint.Messages.Events;

// Published when an order cannot be fulfilled at all - every line was
// out of stock - so OrderAccepted/OrderPlaced never fire. Carries the
// requested items so Finance can mark the order's line items as
// unfulfilled and the cancellation email can list what the customer
// missed out on.
public sealed record OrderCancelled
{
    public required Guid OrderId { get; init; }
    public required List<UnfulfilledItem> UnfulfilledItems { get; init; }
}
