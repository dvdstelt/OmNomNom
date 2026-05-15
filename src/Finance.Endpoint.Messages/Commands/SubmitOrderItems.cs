namespace Finance.Endpoint.Messages.Commands;

public sealed record SubmitOrderItems
{
    public required Guid OrderId { get; init; }
    public required List<OrderItem> Items { get; init; }
}

public sealed record OrderItem
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
