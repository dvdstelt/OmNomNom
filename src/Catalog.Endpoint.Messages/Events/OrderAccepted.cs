namespace Catalog.Endpoint.Messages.Events;

public sealed record OrderAccepted
{
    public required Guid OrderId { get; init; }
}
