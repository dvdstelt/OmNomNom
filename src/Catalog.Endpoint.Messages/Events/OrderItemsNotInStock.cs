namespace Catalog.Endpoint.Messages.Events;

public sealed record OrderItemsNotFulfilled
{
    public required Guid OrderId { get; init; }
    public required List<OrderItem> ItemsNotInStock { get; init; }

    public sealed record OrderItem
    {
        public required Guid ProductId { get; init; }
        public required int QuantityWanted { get; init; }
        public required int QuantityInStock { get; init; }
    }
}
