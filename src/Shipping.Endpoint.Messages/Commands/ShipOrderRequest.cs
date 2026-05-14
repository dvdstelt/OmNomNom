namespace Shipping.Endpoint.Messages.Commands;

public sealed record ShipOrderRequest
{
    public required Guid OrderId { get; init; }
}
