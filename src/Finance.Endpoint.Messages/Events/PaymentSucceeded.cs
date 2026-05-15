namespace Finance.Endpoint.Messages.Events;

public sealed record PaymentSucceeded
{
    public required Guid OrderId { get; init; }
}
