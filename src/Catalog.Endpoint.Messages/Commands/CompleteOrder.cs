namespace Catalog.Endpoint.Messages.Commands;

public sealed record CompleteOrder
{
    public required Guid OrderId { get; init; }
    public required List<OrderItem> Items { get; init; }
}

public sealed record OrderItem
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
