namespace Shipping.Endpoint.Messages.Events;

public sealed record OrderShipped
{
    public required Guid OrderId { get; init; }
}
